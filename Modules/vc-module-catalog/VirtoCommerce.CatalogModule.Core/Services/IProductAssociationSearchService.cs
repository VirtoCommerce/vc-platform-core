using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IProductAssociationSearchService
    {
        GenericSearchResult<CatalogProduct> SearchProductAssociations(ProductAssociationSearchCriteria criteria);
    }
}
