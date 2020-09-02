using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Goke.Students.Server.Data;
using Goke.Students.Server.Models;
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

namespace Goke.Authentication.Extensions.DependencyInjection
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddSwitchAuthentication(this IServiceCollection services)
        {
            // requires using Microsoft.Extensions.Configuration;
            var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var env = services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>();

            var identity = config["Identity"];

            if (identity != "Aspnet")
            {
                ForIdentityServer(services);
            }
            else
            {
                ForIdentity(services);
            }

            return services;
        }

        private static void ForIdentityServer(IServiceCollection services)
        {
            var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var env = services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>();

            if (env.IsDevelopment())
            {
                services.AddIdentityServerDevelopment();
            }
            else
            {
                services.AddIdentityServerProduction(config, env);
            }

            services.AddAuthentication()
                .AddIdentityServerJwt();
        }

        private static void ForIdentity(IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                //options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = false;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = false;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            services.AddControllers().AddNewtonsoftJson();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        public static IApplicationBuilder UseSwitchAuthentication(this IApplicationBuilder app)
        {
            // requires using Microsoft.Extensions.Configuration;
            var config = app.ApplicationServices.GetRequiredService<IConfiguration>();

            var identity = config["Identity"];

            app.Use(async (context, next) =>
            {

                // Do work that doesn't write to the Response.
                var key = "AspnetIdsvr";
                var value = identity;

                var idsvr = context.Request.Cookies["AspnetIdsvr"];
                //if (idsvr == "Aspnet" && identity != "Aspnet")
                if (idsvr != identity)
                {
                        context.Response.Cookies.Delete(".AspNetCore.Identity.Application");
                }

                CookieOptions option = new CookieOptions();
                // option.Expires = DateTime.Now.AddMinutes(10);
                // option.Expires = DateTime.Now.AddMilliseconds(10);

                context.Response.Cookies.Append(key, value, option);

                await next();
                // Do logging or other work that doesn't write to the Response.
            });

            if (identity != "Aspnet")
            {
                app.UseIdentityServer();
            }


            return app;
        }
    }

    public static class IdentityServerExtensions
    {
        public static IIdentityServerBuilder AddIdentityServerDevelopment(this IServiceCollection services)
        {
            return services.AddIdentityServer()
                // .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
                .AddApiAuthorizationWithRole<ApplicationUser, ApplicationDbContext>();

        }

        public static IIdentityServerBuilder AddIdentityServerProduction(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            string filename = null;
            string password = null;
            string thumbprint = null;

            var keyType = config.GetSection("IdentityServer:Key:Type").Value;
            if (keyType == "File")
            {
                filename = Path.Combine(env.ContentRootPath,
                                    config.GetSection("IdentityServer:Key:FilePath").Value);
                password = config.GetSection("IdentityServer:Key:Password").Value;

                Console.WriteLine(filename);
            }
            else
            {
                thumbprint = config.GetSection("IdentityServer:Key:Thumbprint").Value;
            }
            var cert = Goke.Core.Certificate.LoadCert(filename, password, thumbprint);

            return services.AddIdentityServer()
                .AddSigningCredential(cert)
                // .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
                .AddApiAuthorizationWithRole<ApplicationUser, ApplicationDbContext>();

        }

        public static IIdentityServerBuilder AddApiAuthorizationWithRole<TUser, TContext>(this IIdentityServerBuilder builder)
            where TUser : class
            where TContext : DbContext, IPersistedGrantDbContext
        {
            builder.AddApiAuthorization<TUser, TContext>(Role() + Profile());

            // Need to do this as it maps "role" to ClaimTypes.Role and causes issues
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");

            return builder;
        }

        public static IIdentityServerBuilder AddApiAuthorizationWithRole<TUser, TContext>(this IIdentityServerBuilder builder, Action<ApiAuthorizationOptions> configure)
            where TUser : class
            where TContext : DbContext, IPersistedGrantDbContext
        {
            builder.AddApiAuthorization<TUser, TContext>(Role() + Profile() + configure);

            // Need to do this as it maps "role" to ClaimTypes.Role and causes issues
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");

            return builder;
        }

        private static Action<ApiAuthorizationOptions> Role()
        {
            Action<ApiAuthorizationOptions> action = options =>
            {
                // https://github.com/dotnet/AspNetCore.Docs/issues/17649
                options.IdentityResources["openid"].UserClaims.Add("role");
                options.ApiResources.Single().UserClaims.Add("role");
            };
            return action;
        }

        private static Action<ApiAuthorizationOptions> Profile()
        {
            Action<ApiAuthorizationOptions> action = options =>
            {
                options.IdentityResources["profile"].UserClaims.Add("firstname");
                options.IdentityResources["profile"].UserClaims.Add("lastname");
            };
            return action;
        }

       
}
}