using VirtoCommerce.CatalogModule.Core2.Model.ListEntry;
using VirtoCommerce.CatalogModule.Core2.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core2.Services
{
    public interface IListEntrySearchService
	{
		GenericSearchResult<ListEntryBase> Search(CatalogListEntrySearchCriteria criteria);
	}
}
