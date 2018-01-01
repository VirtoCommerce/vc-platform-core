using System.Linq;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Security.Model;

namespace VirtoCommerce.Platform.Security.Repositories
{
    public interface ISecurityRepository
    {
        IQueryable<ApplicationUserEntity> Accounts { get; }
        IQueryable<ApiAccountEntity> ApiAccounts { get; }
        IQueryable<RoleEntity> Roles { get; }
        IQueryable<PermissionEntity> Permissions { get; }
        IQueryable<RoleAssignmentEntity> RoleAssignments { get; }
        IQueryable<RolePermissionEntity> RolePermissions { get; }

        RoleEntity[] GetRolesByIds(string[] ids);
        ApplicationUserEntity GetAccountByName(string userName, UserResponseGroup detailsLevel);
    }
}
