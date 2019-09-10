using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    /// <summary>
    /// Search criteria used for search property dictionary items
    /// </summary>
    public class PropertyDictionaryItemSearchCriteria : SearchCriteriaBase
    {
        public IList<string> PropertyIds { get; set; }

        public string[] CatalogIds { get; set; }
    }
}
