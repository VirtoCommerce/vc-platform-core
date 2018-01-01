using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{
    public interface IRoleManagementService
    {
        GenericSearchResult<Role> SearchRoles(RoleSearchCriteria request);
        IEnumerable<Role> GetRolesByIds(string[] roleIds);
        void DeleteRoles(string[] roleId);
        void SaveRolesChanges(Role[] roles);
    }
}
