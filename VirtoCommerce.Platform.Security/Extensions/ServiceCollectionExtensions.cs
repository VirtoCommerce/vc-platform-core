using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.Events;
using VirtoCommerce.Platform.Core.Security.Search;
using VirtoCommerce.Platform.Security.Handlers;
using VirtoCommerce.Platform.Security.Services;

namespace VirtoCommerce.Platform.Security.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurityServices(this IServiceCollection services, Action<SecurityOptions> setupAction = null)
        {
            services.AddScoped<IUserNameResolver, HttpContextUserResolver>();
            services.AddSingleton<IKnownPermissionsProvider, DefaultPermissionProvider>();
            services.AddScoped<IRoleSearchService, RoleSearchService>();
            services.AddScoped<IUserSearchService, UserSearchService>();
            //Identity dependencies override
            services.TryAddScoped<RoleManager<Role>, CustomRoleManager>();
            services.TryAddScoped<UserManager<ApplicationUser>, CustomUserManager>();

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            var providerSnapshot = services.BuildServiceProvider();
            var inProcessBus = providerSnapshot.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<UserChangedEvent>(async (message, token) => await providerSnapshot.GetService<LogChangesUserChangedEventHandler> ().Handle(message));
            inProcessBus.RegisterHandler<UserPasswordChangedEvent>(async (message, token) => await providerSnapshot.GetService<LogChangesUserChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<UserResetPasswordEvent>(async (message, token) => await providerSnapshot.GetService<LogChangesUserChangedEventHandler>().Handle(message));

            return services;
        }
    }
}
