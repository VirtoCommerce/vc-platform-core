using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Security.Caching;

namespace VirtoCommerce.Platform.Security.Services
{
    public class CustomRoleManager : AspNetRoleManager<Role>
    {
        private readonly IKnownPermissionsProvider _knownPermissions;
        private readonly IMemoryCache _memoryCache;
        public CustomRoleManager(IKnownPermissionsProvider knownPermissions, IMemoryCache memoryCache, IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators,
                                 ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger, IHttpContextAccessor contextAccessor)
            : base(store, roleValidators, keyNormalizer, errors, logger, contextAccessor)
        {
            _knownPermissions = knownPermissions;
            _memoryCache = memoryCache;
        }

        public override async Task<Role> FindByNameAsync(string roleName)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByNameAsync", roleName);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken());
                var role = await base.FindByNameAsync(roleName);
                if (role != null)
                {
                    await LoadRolePermissionsAsync(role);
                }
                return role;
            }, cacheNullValue: false);
            return result;
        }

        public override async Task<Role> FindByIdAsync(string roleId)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByIdAsync", roleId);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken());
                var role = await base.FindByIdAsync(roleId);
                if (role != null)
                {
                    await LoadRolePermissionsAsync(role);
                }
                return role;
            }, cacheNullValue: false);
            return result;
        }

        public override async Task<IdentityResult> CreateAsync(Role role)
        {
            var result = await base.CreateAsync(role);
            if (result.Succeeded && !role.Permissions.IsNullOrEmpty())
            {
                var permissionRoleClaims = role.Permissions.Select(x => new Claim(SecurityConstants.Claims.PermissionClaimType, x.Name));
                foreach (var claim in permissionRoleClaims)
                {
                    await base.AddClaimAsync(role, claim);
                }
                SecurityCacheRegion.ExpireRegion();
            }
            return result;
        }

        public override async Task<IdentityResult> UpdateAsync(Role role)
        {
            //TODO: Unstable method work, sometimes throws EF already being tracked exception 
            //https://github.com/aspnet/Identity/issues/1807
            var result = await base.UpdateAsync(role);
            if (result.Succeeded && role.Permissions != null)
            {
                var sourcePermissionClaims = role.Permissions.Select(x => new Claim(SecurityConstants.Claims.PermissionClaimType, x.Name)).ToList();
                var targetPermissionClaims = (await GetClaimsAsync(role)).Where(x => x.Type == SecurityConstants.Claims.PermissionClaimType).ToList();
                var comparer = AnonymousComparer.Create((Claim x) => x.Value);
                //Add
                foreach (var sourceClaim in sourcePermissionClaims.Except(targetPermissionClaims, comparer))
                {
                    await base.AddClaimAsync(role, sourceClaim);
                }
                //Remove
                foreach (var targetClaim in targetPermissionClaims.Except(sourcePermissionClaims, comparer).ToArray())
                {
                    await base.RemoveClaimAsync(role, targetClaim);
                }

                SecurityCacheRegion.ExpireRegion();
            }
            return result;
        }

        public override async Task<IdentityResult> DeleteAsync(Role role)
        {
            var result = await base.DeleteAsync(role);
            if (result.Succeeded)
            {
                SecurityCacheRegion.ExpireRegion();
            }
            return result;
        }

        protected virtual async Task LoadRolePermissionsAsync(Role role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (SupportsRoleClaims)
            {
                //Load role claims and convert it to the permissions and assign to role
                var rolePermissionClaims = (await GetClaimsAsync(role)).Where(x => x.Type == SecurityConstants.Claims.PermissionClaimType);
                role.Permissions = _knownPermissions.GetAllPermissions().Join(rolePermissionClaims, p => p.Name, c => c.Value, (p, c) => p).ToList();
            }
        }
    }
}
