using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Data.Search.BrowseFilters
{
    public interface IBrowseFilter
    {
        string Key { get; }
        int Order { get; }
        IList<IBrowseFilterValue> GetValues();
    }
}
