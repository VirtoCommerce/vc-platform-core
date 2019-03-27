using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model.ListEntry;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    public class ListEntrySearchResult : GenericSearchResult<ListEntryBase>
    {

        /// <summary>
        /// Gets or sets the list entries.
        /// </summary>
        /// <value>
        /// The list entries.
        /// </value>
		public IList<ListEntryBase> ListEntries => Results;
    }
}
