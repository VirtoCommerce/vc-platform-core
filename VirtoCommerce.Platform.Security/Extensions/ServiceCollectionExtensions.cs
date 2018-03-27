using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.Search;
using VirtoCommerce.Platform.Security.Services;

namespace VirtoCommerce.Platform.Security.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurityServices(this IServiceCollection services)
        {
            services.AddScoped<IUserNameResolver, HttpContextUserResolver>();
            services.AddSingleton<IKnownPermissionsProvider, DefaultPermissionProvider>();
            services.AddScoped<IRoleSearchService, RoleSearchService>();
            services.AddScoped<IUserSearchService, UserSearchService>();
            return services;
        }
    }
}
