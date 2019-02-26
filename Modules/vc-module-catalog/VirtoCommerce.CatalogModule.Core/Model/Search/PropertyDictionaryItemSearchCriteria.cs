using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    /// <summary>
    /// Search criteria used for search property dictionary items
    /// </summary>
    public class PropertyDictionaryItemSearchCriteria : SearchCriteriaBase
    {
        public IList<string> PropertyIds { get; set; }
    }
}
