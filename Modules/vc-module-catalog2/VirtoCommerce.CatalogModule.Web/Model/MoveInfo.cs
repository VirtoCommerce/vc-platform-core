using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core2.Model.ListEntry;

namespace VirtoCommerce.CatalogModule.Web2.Model
{
    /// <summary>
    /// Represent move operation detail
    /// </summary>
    public class MoveInfo
    {
        public string Catalog { get; set; }
        public string Category { get; set; }

        public IList<ListEntryBase> ListEntries { get; set; }
    }
}
