using System.Threading.Tasks;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;

namespace VirtoCommerce.StoreModule.Core.Services
{
	public interface IStoreService
	{
        Task<StoreSearchResult> SearchStoresAsync(StoreSearchCriteria criteria);
        Task<Store> GetByIdAsync(string id);
	    Task<Store[]> GetByIdsAsync(string[] ids);
        Task<Store> CreateAsync(Store store);
		Task UpdateAsync(Store[] stores);
		Task DeleteAsync(string[] ids);

        //TODO
        /// <summary>
        /// Returns list of stores ids which passed user can signIn
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //IEnumerable<string> GetUserAllowedStoreIds(ApplicationUserExtended user);
    }
}
