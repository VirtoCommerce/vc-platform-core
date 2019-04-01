
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class CategoryLink : ValueObject
    {
        /// <summary>
        /// Entry identifier which this link belongs to
        /// </summary>
        public string EntryId => ListEntryId;
        public string ListEntryId { get; set; }

        /// <summary>
        /// Gets or sets the type of the list entry. E.g. "product", "category"
        /// </summary>
        /// <value>
        /// The type of the list entry.
        /// </value>
        public string ListEntryType { get; set; }

        /// <summary>
        /// Product order position in virtual catalog
        /// </summary>
        public int Priority { get; set; }

        public string CatalogId { get; set; }
        [JsonIgnore]
        public Catalog Catalog { get; set; }
        public string CategoryId { get; set; }
        public Category Category { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return EntryId;
            yield return CatalogId;
            yield return CategoryId;
        }
    }
}
