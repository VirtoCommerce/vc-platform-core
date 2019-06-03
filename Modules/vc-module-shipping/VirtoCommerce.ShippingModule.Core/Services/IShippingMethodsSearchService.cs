using System.Threading.Tasks;
using VirtoCommerce.ShippingModule.Core.Model.Search;

namespace VirtoCommerce.ShippingModule.Core.Services
{
    public interface IShippingMethodsSearchService
    {
        Task<ShippingMethodsSearchResult> SearchShippingMethodsAsync(ShippingMethodsSearchCriteria criteria);
    }
}
