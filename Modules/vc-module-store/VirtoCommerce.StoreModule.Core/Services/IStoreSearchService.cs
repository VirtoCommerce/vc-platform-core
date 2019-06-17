using System.Threading.Tasks;
using VirtoCommerce.StoreModule.Core.Model.Search;

namespace VirtoCommerce.StoreModule.Core.Services
{
    public interface IStoreSearchService
    {
        Task<StoreSearchResult> SearchStoresAsync(StoreSearchCriteria criteria);
    }
}
