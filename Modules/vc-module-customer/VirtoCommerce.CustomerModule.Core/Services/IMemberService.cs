using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.CustomerModule.Core.Services
{
    /// <summary>
    /// Abstraction for member CRUD operations
    /// </summary>
    public interface IMemberService
    {
        Task<Member[]> GetByIdsAsync(string[] memberIds, string responseGroup = null, string[] memberTypes = null);
        Task<Member> GetByIdAsync(string memberId, string responseGroup = null, string memberType = null);
        Task SaveChangesAsync(Member[] members);
        Task DeleteAsync(string[] ids, string[] memberTypes = null);
    }
}
