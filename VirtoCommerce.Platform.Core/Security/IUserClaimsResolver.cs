using System.Security.Claims;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Security
{
    public interface IUserClaimsResolver
    {
        Task<ClaimsPrincipal> GetUserClaims(string userName);
    }
}
