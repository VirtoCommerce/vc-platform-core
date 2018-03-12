using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface ICatalogSearchService
	{
		SearchResult Search(SearchCriteria criteria);
	}
}
