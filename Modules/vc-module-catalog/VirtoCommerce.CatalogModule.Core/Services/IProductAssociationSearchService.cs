using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IProductAssociationSearchService
    {
        GenericSearchResult<ProductAssociation> SearchProductAssociations(ProductAssociationSearchCriteria criteria);
    }
}
