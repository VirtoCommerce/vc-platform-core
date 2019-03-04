using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    /// <summary>
    /// Represent move operation detail
    /// </summary>
    public class MoveInfo
    {
        public string Catalog { get; set; }
        public string Category { get; set; }

        public ICollection<ListEntry> ListEntries { get; set; }
    }
}
