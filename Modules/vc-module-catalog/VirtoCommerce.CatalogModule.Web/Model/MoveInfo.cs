using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.CatalogModule.Core.Model.ListEntry;

namespace VirtoCommerce.CatalogModule.Web.Model
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
