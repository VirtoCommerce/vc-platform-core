using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.StoreModule.Core.Services
{
    public interface IStoreService
    {
        Task<Store[]> GetByIdsAsync(string[] ids, string responseGroup = null);
        Task<Store> GetByIdAsync(string id, string responseGroup = null);
        Task SaveChangesAsync(Store[] stores);
        Task DeleteAsync(string[] ids);

        /// <summary>
        /// Returns list of stores ids which passed user can signIn
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetUserAllowedStoreIdsAsync(ApplicationUser user);
    }
}
