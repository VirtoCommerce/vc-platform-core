using System.Threading.Tasks;
using VirtoCommerce.TaxModule.Core.Model.Search;

namespace VirtoCommerce.TaxModule.Core.Services
{
    public interface ITaxProviderSearchService
    {
        Task<TaxProviderSearchResult> SearchTaxProvidersAsync(TaxProviderSearchCriteria criteria);
    }
}
