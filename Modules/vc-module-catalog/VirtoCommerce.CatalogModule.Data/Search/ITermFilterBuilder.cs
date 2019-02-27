using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public interface ITermFilterBuilder
    {
        FiltersContainer GetTermFilters(ProductSearchCriteria criteria);
    }
}
