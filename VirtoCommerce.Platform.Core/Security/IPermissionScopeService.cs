using System;
using System.Collections.Generic;

namespace VirtoCommerce.Platform.Core.Security
{
    public interface IPermissionScopeRequirementService
    {
        /// <summary>
        /// Return scopes list for concrete permission used in future for permission bound
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        IEnumerable<PermissionScopeRequirement> GetAvailablePermissionScopes(string permission);
        /// <summary>
        /// Factory method for scope
        /// </summary>
        /// <param name="scopeTypeName"></param>
        /// <returns></returns>
        PermissionScopeRequirement CreateScopeByTypeName(string scopeTypeName);
        /// <summary>
        /// Gets concrete entity scope resulting strings representation for using in future permission checks 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        IEnumerable<string> GetObjectPermissionScopeStrings(object obj);

        void RegisterScope(Func<PermissionScopeRequirement> scopeFactory);
    }
}
