using VirtoCommerce.CatalogModule.Core2.Model;
using VirtoCommerce.CatalogModule.Core2.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core2.Services
{
    public interface IProductAssociationSearchService
    {
        GenericSearchResult<CatalogProduct> SearchProductAssociations(ProductAssociationSearchCriteria criteria);
    }
}
