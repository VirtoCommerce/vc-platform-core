using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web.Model
{
    /// <summary>
    /// Represents  move list entries command
    /// </summary>
    public class ListEntriesMoveRequest : ValueObject, IHasCatalogId
    {
        //Destination catalog
        public string Catalog { get; set; }
        public string CatalogId => Catalog;
        //Destination category
        public string Category { get; set; }

        public ICollection<Core.Model.ListEntry.ListEntryBase> ListEntries { get; set; }
    }
}
