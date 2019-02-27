using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public class CategorySearchService : CatalogSearchService<Category, CategorySearchCriteria, CategorySearchResult>, ICategorySearchService
    {
        private readonly ICategoryService _categoryService;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public CategorySearchService(ISearchRequestBuilder[] searchRequestBuilders, ISearchProvider searchProvider, ISettingsManager settingsManager, ICategoryService categoryService, IBlobUrlResolver blobUrlResolver)
            : base(searchRequestBuilders, searchProvider, settingsManager)
        {
            _categoryService = categoryService;
            _blobUrlResolver = blobUrlResolver;
        }

        protected override async Task<IList<Category>> LoadMissingItems(string[] missingItemIds, CategorySearchCriteria criteria)
        {
            var catalog = criteria?.CatalogId;
            var responseGroup = GetResponseGroup(criteria);

            var categories = await _categoryService.GetByIdsAsync(missingItemIds, responseGroup, catalog);

            var result = categories.Select(p => p.ToWebModel(_blobUrlResolver)).ToArray();
            return result;
        }

        protected override void ReduceSearchResults(IEnumerable<Category> items, CategorySearchCriteria criteria)
        {
        }

        protected override Aggregation[] ConvertAggregations(IList<AggregationResponse> aggregationResponses, CategorySearchCriteria criteria)
        {
            return null;
        }


        protected virtual CategoryResponseGroup GetResponseGroup(CategorySearchCriteria criteria)
        {
            var result = EnumUtility.SafeParse(criteria?.ResponseGroup, CategoryResponseGroup.Full & ~CategoryResponseGroup.WithProperties);
            return result;
        }
    }
}
