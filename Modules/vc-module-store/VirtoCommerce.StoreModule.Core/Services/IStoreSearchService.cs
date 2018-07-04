using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;

namespace VirtoCommerce.StoreModule.Core.Services
{
    public interface IStoreSearchService
    {
        Task<GenericSearchResult<Store>> SearchStoresAsync(StoreSearchCriteria criteria);
    }
}
