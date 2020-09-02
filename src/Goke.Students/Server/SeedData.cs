using Goke.Students.Server.Data;
using Goke.Students.Server.Models;
using Goke.Students.Shared;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Goke.Students.Server
{
    internal class SeedData
    {
        internal static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration config)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>(),
                serviceProvider.GetRequiredService<IOptions<OperationalStoreOptions>>()
                ))
            {
                context.Database.Migrate();

                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                // allowed user can create and edit contacts that they create
                var userName = config["Seed:AdminUN"];
                var userPw = config["Seed:AdminPW"];
                var userFN = config["Seed:AdminFN"];
                var userLN = config["Seed:AdminLN"];

                var userID = await EnsureUser(serviceProvider, userPw, userName, userFN, userLN);

                var userRoles = config["Seed:AdminR"]?.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var role in userRoles)
                {
                    await EnsureRole(serviceProvider, userID, role);
                }
            }
        }

        private static void SeedDB(ApplicationDbContext context, string adminID)
        {
            throw new NotImplementedException();
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, string UserName, string firstName, string lastName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(UserName);
                       

            if (user == null)
            {
                user = await CreateUserAsync(testUserPw, UserName, firstName, lastName, userManager);
            }
            else
            {
                var IR = await userManager.DeleteAsync(user);
                if (IR.Succeeded)
                {
                    user = await CreateUserAsync(testUserPw, UserName, firstName, lastName, userManager);
                }
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        protected static async Task<ApplicationUser> CreateUserAsync(string testUserPw, string UserName, string firstName, string lastName, UserManager<ApplicationUser> userManager)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = UserName,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName,
            };
            await userManager.CreateAsync(user, testUserPw);
            return user;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
    }
}