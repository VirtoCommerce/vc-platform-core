using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.Search.Indexing
{
    public class CategoryIndexedSearchService : CatalogIndexedSearchService<Category, CategoryIndexedSearchCriteria, CategoryIndexedSearchResult>, ICategoryIndexedSearchService
    {
        private readonly ICategoryService _categoryService;

        public CategoryIndexedSearchService(IEnumerable<ISearchRequestBuilder> searchRequestBuilders, ISearchProvider searchProvider, ISettingsManager settingsManager, ICategoryService categoryService)
            : base(searchRequestBuilders, searchProvider, settingsManager)
        {
            _categoryService = categoryService;
        }

        protected override async Task<IList<Category>> LoadMissingItems(string[] missingItemIds, CategoryIndexedSearchCriteria criteria)
        {
            var catalog = criteria?.CatalogId;
            var responseGroup = GetResponseGroup(criteria);

            var categories = await _categoryService.GetByIdsAsync(missingItemIds, responseGroup.ToString(), catalog);

            return categories;
        }

        protected override void ReduceSearchResults(IEnumerable<Category> items, CategoryIndexedSearchCriteria criteria)
        {
        }

        protected override Task<Aggregation[]> ConvertAggregationsAsync(IList<AggregationResponse> aggregationResponses, CategoryIndexedSearchCriteria criteria)
        {
            return Task.FromResult(Array.Empty<Aggregation>());
        }


        protected virtual CategoryResponseGroup GetResponseGroup(CategoryIndexedSearchCriteria criteria)
        {
            var result = EnumUtility.SafeParseFlags(criteria?.ResponseGroup, CategoryResponseGroup.Full & ~CategoryResponseGroup.WithProperties);
            return result;
        }
    }
}
