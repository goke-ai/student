using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using Goke.Students.Client.States;
using Goke.Students.Client.Services.Contracts;
using Goke.Students.Client.Services.Implementations;
using Goke.Students.Client.Data;
using Microsoft.Extensions.Localization;

namespace Goke.Students.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            var identity = builder.Configuration["Identity"];

            builder.RootComponents.Add<App>("app");

            if (identity != "Aspnet")
            {
                builder.Services.AddHttpClient("Goke.Students.ServerAPI", 
                    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

                // Other DI services
                builder.Services.AddScoped<LocalPeopleStore>(); 
                // Supply HttpClient instances that include access tokens when making requests to the server project
                builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Goke.Students.ServerAPI"));

                
                builder.Services.AddApiAuthorization();
                builder.Services.AddApiAuthorization(options =>
                {
                    options.UserOptions.RoleClaim = "role";
                });
            }
            else
            {
                // Other DI services
                builder.Services.AddScoped<LocalPeopleStore>(); 
                
                builder.Services.AddOptions();
                builder.Services.AddAuthorizationCore();
                builder.Services.AddScoped<IdentityAuthenticationStateProvider>();
                builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<IdentityAuthenticationStateProvider>());
                builder.Services.AddScoped<IAuthorizeApi, AuthorizeApi>();

                builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            }

            // builder.Services.AddScoped<AccountClaimsPrincipalFactory<RemoteUserAccount>, OfflineAccountClaimsPrincipalFactory>();

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            // builder.Services.AddTransient<IStringLocalizer, StringLocalizer>()

            await builder.Build().RunAsync();
        }
    }
}
