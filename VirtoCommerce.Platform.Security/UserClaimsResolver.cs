using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Security
{
    public class UserClaimsResolver : IUserClaimsResolver
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserClaimsResolver(IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, UserManager<ApplicationUser> userManager)
        {
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> GetUserClaims(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var result = await _userClaimsPrincipalFactory.CreateAsync(user);

            return result;
        }
    }
}
