using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Search
{
    public interface IBrowseFilter
    {
        string Key { get; }
        int Order { get; }
        IList<IBrowseFilterValue> GetValues();
    }
}
