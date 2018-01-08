using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Core;
using OpenIddict.Models;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Security
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UsePlatformPermissions(this IApplicationBuilder appBuilder)
        {
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsProvider>();
            permissionsProvider.RegisterPermissions(SecurityConstants.Permissions.AllPermissions.Select(x => new Permission() { Name = x }).ToArray());
            return appBuilder;
        }


        public static async Task<IApplicationBuilder> UseDefaultUsersAsync(this IApplicationBuilder appBuilder)
        {
            using (var scope = appBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                var manager = scope.ServiceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictApplication>>();

                var administratorRole = new Role { Name = "Administrator" };
                if(!(await roleManager.RoleExistsAsync(administratorRole.Name)))
                {
                    await roleManager.CreateAsync(administratorRole);
                }
                var admin = new ApplicationUser
                {
                    Id = "1eb2fa8ac6574541afdb525833dadb46",
                    UserName = "admin",
                    Email = "admin@email.com",
                    PasswordHash = "AHQSmKnSLYrzj9vtdDWWnUXojjpmuDW2cHvWloGL9UL3TC9UCfBmbIuR2YCyg4BpNg=="
                };
                var adminUser = await userManager.FindByIdAsync(admin.Id);
                if (adminUser == null)
                {
                    var result = await userManager.CreateAsync(admin);
                    adminUser = await userManager.FindByIdAsync(admin.Id);
                    await userManager.AddToRoleAsync(admin, administratorRole.Name);
                }

                if (await manager.FindByClientIdAsync("manager-ui", CancellationToken.None) == null)
                {
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = "manager-ui",
                        DisplayName = "Platform manager application"                 
                    };
                    await manager.CreateAsync(descriptor, CancellationToken.None);
                }

            }
            return appBuilder;
        }
    }
}
