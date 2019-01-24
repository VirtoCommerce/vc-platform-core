using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Security.Converters
{
    public static class PermissionClaimConverter
    {
        public static Permission FromClaim(this Claim claim, IPermissionScopeRequirementService permissionScopeService)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            if (!claim.Type.EqualsInvariant(PlatformConstants.Security.Claims.PermissionClaimType))
            {
                throw new ArgumentException($"Claim should have type \"{PlatformConstants.Security.Claims.PermissionClaimType}\" to be a permission.");
            }

            var permission = JsonConvert.DeserializeObject<Permission>(claim.Value, GetJsonSerializerSettings());
            permission.AvailableScopes = permissionScopeService.GetAvailablePermissionScopes(permission.Name).ToList();
            return permission;
        }

        public static Claim ToClaim(this Permission permission, IPermissionScopeRequirementService permissionScopeService)
        {
            if (permission == null)
            {
                throw new ArgumentNullException(nameof(permission));

            }
            var serializerSettings = GetJsonSerializerSettings();
            serializerSettings.ContractResolver = new PermissionContractResolver();
            permission.AssignedScopes = permission.AssignedScopes
                .Select(x =>
                {
                    var specificTypeScope = permissionScopeService.CreateScopeByTypeName(x.Type);
                    x.Patch(specificTypeScope);
                    return specificTypeScope;
                })
                .ToList();
            return new Claim(PlatformConstants.Security.Claims.PermissionClaimType, JsonConvert.SerializeObject(permission, serializerSettings));
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                // Important - to serialize/deserialize specific implementations of PermissionScopes, not just base class
                TypeNameHandling = TypeNameHandling.Auto,
                // To avoid conflicts with version numbers with updated version
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                NullValueHandling = NullValueHandling.Ignore,
            };
        }
    }

    /// <summary>
    /// Custom JSON serializer that skips Permission.AvailableScopes property
    /// </summary>
    public class PermissionContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).Where(p => !p.PropertyName.Equals(nameof(Permission.AvailableScopes))).ToList();
        }
    }
}
