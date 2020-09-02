using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goke.Students.Server.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Goke.Students.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            Seeding(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        protected static void Seeding(IHost host)
        {
            // Initialize the database
            var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var message = "DB seeded successfully.";
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();

                    // requires using Microsoft.Extensions.Configuration;
                    var config = host.Services.GetRequiredService<IConfiguration>();
                    // Set password with the Secret Manager tool.
                    // dotnet user-secrets set SeedUserPW <pw>

                    // var testUserPw = config["SeedUserPW"];
                    SeedData.InitializeAsync(scope.ServiceProvider, config).Wait();

                    // Ark.Coeus.Data.Seeding.Start(serviceProvider.GetRequiredService<AppDbContext>(), serviceProvider.GetRequiredService<ILogger<Ark.Coeus.Data.Seeding>>());

                    logger.LogInformation(message);
                }
                catch (Exception ex)
                {
                    message = "An error occurred seeding the DB.";
                    logger.LogError(ex, message);
                    Console.WriteLine(message);
                }

            }

            //using (var scope = host.Services.CreateScope())
            //{
            //    var serviceProvider = scope.ServiceProvider;

            //    try
            //    {
            //        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            //        context.Database.Migrate();

            //        // requires using Microsoft.Extensions.Configuration;
            //        var config = host.Services.GetRequiredService<IConfiguration>();
            //        // Set password with the Secret Manager tool.
            //        // dotnet user-secrets set SeedUserPW <pw>

            //        // var testUserPw = config["SeedUserPW"];

            //        SeedData.InitializeAsync(serviceProvider, config).Wait();

            //        // Ark.Coeus.Data.Seeding.Start(serviceProvider.GetRequiredService<AppDbContext>(), serviceProvider.GetRequiredService<ILogger<Ark.Coeus.Data.Seeding>>());
            //    }
            //    catch (Exception ex)
            //    {
            //        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            //        logger.LogError(ex, "An error occurred seeding the DB.");
            //    }
            //}
        }

    }
}
