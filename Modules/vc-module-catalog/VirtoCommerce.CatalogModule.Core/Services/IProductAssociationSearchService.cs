using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Model.Search;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Domain.Catalog.Services
{
    public interface IProductAssociationSearchService
    {
        GenericSearchResult<CatalogProduct> SearchProductAssociations(ProductAssociationSearchCriteria criteria);
    }
}
