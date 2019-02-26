using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class PropertyDictionaryItem : Entity
    {
        public string PropertyId { get; set; }
        public string Alias { get; set; }
        public int SortOrder { get; set; }
        public ICollection<PropertyDictionaryItemLocalizedValue> LocalizedValues { get; set; }
    }
}
