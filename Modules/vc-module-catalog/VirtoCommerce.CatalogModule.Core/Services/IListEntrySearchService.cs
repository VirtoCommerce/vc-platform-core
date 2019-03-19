using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.ListEntry;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IListEntrySearchService
	{
		GenericSearchResult<ListEntryBase> Search(CatalogListEntrySearchCriteria criteria);
	}
}
