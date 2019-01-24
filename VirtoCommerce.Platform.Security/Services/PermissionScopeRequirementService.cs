using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Security.Services
{
    public class PermissionScopeRequirementService : IPermissionScopeRequirementService
    {
        private readonly Dictionary<PermissionScopeRequirement, Func<PermissionScopeRequirement>> _scopeFactories = new Dictionary<PermissionScopeRequirement, Func<PermissionScopeRequirement>>();

        #region ISecurityScopeService Members
        public IEnumerable<PermissionScopeRequirement> GetAvailablePermissionScopes(string permission)
        {
            return _scopeFactories.Keys.Where(x => x.IsScopeAvailableForPermission(permission)).ToArray();
        }

        public PermissionScopeRequirement CreateScopeByTypeName(string scopeTypeName)
        {
            if (scopeTypeName == null)
            {
                throw new ArgumentNullException(nameof(scopeTypeName));
            }
            var scopeKey = _scopeFactories.Keys.FirstOrDefault(x => scopeTypeName.EqualsInvariant(x.Type)) ?? throw new KeyNotFoundException($"Scope type \"{scopeTypeName}\" was not registered in PermissionScopeService.");
            return _scopeFactories[scopeKey]();
        }

        public IEnumerable<string> GetObjectPermissionScopeStrings(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            return _scopeFactories.Keys.SelectMany(x => x.GetEntityScopeStrings(obj)).Where(x => !string.IsNullOrEmpty(x));
        }

        public void RegisterScope(Func<PermissionScopeRequirement> scopeFactory)
        {
            if (scopeFactory == null)
            {
                throw new ArgumentNullException(nameof(scopeFactory));
            }
            _scopeFactories.Add(scopeFactory(), scopeFactory);
        }
        #endregion
    }
}
