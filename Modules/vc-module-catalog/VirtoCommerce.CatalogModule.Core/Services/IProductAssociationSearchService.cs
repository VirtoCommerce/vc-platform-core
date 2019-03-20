using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IProductAssociationSearchService
    {
        Task<GenericSearchResult<ProductAssociation>> SearchProductAssociationsAsync(ProductAssociationSearchCriteria criteria);
    }
}
