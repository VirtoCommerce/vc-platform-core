using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{
    /// <summary>
    /// Base class for all types repesents permision boundary scopes
    /// </summary>
    public class PermissionScopeRequirement : ValueObject, IAuthorizationRequirement
    {
        public PermissionScopeRequirement()
        {
            Type = GetType().Name;
        }
        /// <summary>
        /// Define scope type influences the choice of ui pattern in role definition 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Display representation 
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Represent string scope value
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Permission we want to get with the requirement
        /// </summary>
        [JsonIgnore]
        public string DesiredPermission { get; set; }

        /// <summary>
        /// Checks if scope could be bounded to permission.
        /// </summary>
        /// <param name="permission">Permission.</param>
        /// <returns>True if that scope could be bounded to given permission.</returns>
        public virtual bool IsScopeAvailableForPermission(string permission)
        {
            return GetAvailablePermissions().Any(x => x.EqualsInvariant(permission));
        }

        /// <summary>
        /// Returns all supported permissions for this scope.
        /// </summary>
        /// <returns>Supported permissions.</returns>
        public virtual IEnumerable<string> GetAvailablePermissions()
        {
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Returns resulting list of scope strings for entity that may be used for permissions check
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual IEnumerable<string> GetEntityScopeStrings(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            foreach (var kvp in GetSupportedEntityProviders())
            {
                var entityType = kvp.Key;
                var idProvider = kvp.Value;
                if (entity.GetType().IsDerivativeOf(entityType))
                {
                    yield return Type + ":" + idProvider(entity);
                }
            }
        }

        /// <summary>
        /// Gets Id providers for all entity types on which that permission scope could be checked.
        /// </summary>
        /// <returns>Dictionary with entity type keys and idProvider values.</returns>
        public virtual Dictionary<Type, Func<object, string>> GetSupportedEntityProviders()
        {
            return new Dictionary<Type, Func<object, string>>();
        }

        public override string ToString()
        {
            return Type + ":" + Scope;
        }

        public virtual void Patch(PermissionScopeRequirement target)
        {
            target.Type = Type;
            target.Label = Label;
            target.Scope = Scope;
        }
    }
}
