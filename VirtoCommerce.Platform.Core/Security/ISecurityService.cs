using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{
    public interface ISecurityService
    {
        Task<ApplicationUser> FindByNameAsync(string userName, UserResponseGroup detailsLevel);
        Task<ApplicationUser> FindByIdAsync(string userId, UserResponseGroup detailsLevel);
        Task<ApplicationUser> FindByEmailAsync(string email, UserResponseGroup detailsLevel);
        Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, UserResponseGroup detailsLevel);
        Task<SecurityResult> CreateAsync(ApplicationUser user);
        Task<SecurityResult> UpdateAsync(ApplicationUser user);
        Task DeleteAsync(string[] names);
        ApiAccount GenerateNewApiAccount(ApiAccountType type);
        ApiAccount GenerateNewApiKey(ApiAccount account);
        Task<string> GeneratePasswordResetTokenAsync(string userId);
        Task<SecurityResult> ChangePasswordAsync(string name, string oldPassword, string newPassword);
        Task<SecurityResult> ResetPasswordAsync(string name, string newPassword);
        Task<SecurityResult> ResetPasswordAsync(string userId, string token, string newPassword);
        Task<GenericSearchResult<ApplicationUser>> SearchUsersAsync(UserSearchCriteria request);
        bool UserHasAnyPermission(string userName, string[] scopes, params string[] permissionIds);
        Permission[] GetAllPermissions();
        Permission[] GetUserPermissions(string userName);
        Task<bool> IsUserLockedAsync(string userId);
        Task<SecurityResult> UnlockUserAsync(string userId);
    }
}
