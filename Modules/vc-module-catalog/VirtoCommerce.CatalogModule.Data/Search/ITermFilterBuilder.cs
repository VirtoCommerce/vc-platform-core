using VirtoCommerce.Domain.Catalog.Model.Search;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public interface ITermFilterBuilder
    {
        FiltersContainer GetTermFilters(ProductSearchCriteria criteria);
    }
}
