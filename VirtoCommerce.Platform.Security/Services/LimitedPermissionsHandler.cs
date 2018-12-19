using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Security.Services
{
    public class LimitedPermissionsHandler
    {
        public const string LimitedPermissionsClaimName = "LimitedPermissions";

        public virtual Task<bool> UserHasAnyPermissionAsync(IReadOnlyCollection<Claim> claims, string requiredPermission)
        {
            var limitedPermissionsClaim = claims.FirstOrDefault(c => c.Type.EqualsInvariant(LimitedPermissionsClaimName));
            if (limitedPermissionsClaim == null)
            {
                return Task.Run(() => false);
            }

            var limitedPermissions = limitedPermissionsClaim.Value?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            return Task.Run(() => limitedPermissions.Contains(requiredPermission));

        }

        public virtual bool HasLimitedPermissionsClaim(IReadOnlyCollection<Claim> claims)
        {
            return claims.Any(c => c.Type.EqualsInvariant(LimitedPermissionsClaimName));
        }
    }
}
