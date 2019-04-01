using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    public class PropertySearchCriteria : SearchCriteriaBase
    {
        public string CatalogId { get; set; }
        public IList<string> PropertyNames { get; set; }
    }
}
