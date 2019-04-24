using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web.Model
{
    /// <summary>
    /// Represent move operation detail
    /// </summary>
    public class MoveInfo : ValueObject
    {
        public string Catalog { get; set; }
        public string Category { get; set; }

        public ICollection<Core.Model.ListEntry.ListEntryBase> ListEntries { get; set; }
    }
}
