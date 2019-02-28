using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public abstract class CatalogSearchService<TItem, TCriteria, TResult>
        where TItem : Entity
        where TCriteria : SearchCriteriaBase
        where TResult : CatalogSearchResult<TItem>
    {
        private readonly ISearchRequestBuilder[] _searchRequestBuilders;
        private readonly ISearchProvider _searchProvider;
        private readonly ISettingsManager _settingsManager;

        protected CatalogSearchService(ISearchRequestBuilder[] searchRequestBuilders, ISearchProvider searchProvider, ISettingsManager settingsManager)
        {
            _searchRequestBuilders = searchRequestBuilders;
            _searchProvider = searchProvider;
            _settingsManager = settingsManager;
        }

        public virtual async Task<TResult> SearchAsync(TCriteria criteria)
        {
            var result = AbstractTypeFactory<TResult>.TryCreateInstance();

            var requestBuilder = GetRequestBuilder(criteria);
            var request = await requestBuilder?.BuildRequestAsync(criteria);

            var response = await _searchProvider.SearchAsync(criteria.ObjectType, request);

            if (response != null)
            {
                result.TotalCount = response.TotalCount;
                result.Items = await ConvertDocuments(response.Documents, criteria);
                result.Aggregations = await ConvertAggregationsAsync(response.Aggregations, criteria);
            }

            return result;
        }


        protected abstract Task<IList<TItem>> LoadMissingItems(string[] missingItemIds, TCriteria criteria);
        protected abstract void ReduceSearchResults(IEnumerable<TItem> items, TCriteria criteria);
        protected abstract Task<Aggregation[]> ConvertAggregationsAsync(IList<AggregationResponse> aggregationResponses, TCriteria criteria);


        protected virtual ISearchRequestBuilder GetRequestBuilder(TCriteria criteria)
        {
            if (_searchRequestBuilders == null)
                throw new InvalidOperationException("No query builders defined");

            var queryBuilder = _searchRequestBuilders.SingleOrDefault(b => b.DocumentType.Equals(criteria.ObjectType)) ??
                               _searchRequestBuilders.First(b => b.DocumentType.Equals(string.Empty));

            return queryBuilder;
        }

        protected virtual async Task<TItem[]> ConvertDocuments(IList<SearchDocument> documents, TCriteria criteria)
        {
            TItem[] result = null;

            if (documents?.Any() == true)
            {
                var returnObjectsFromIndex = _settingsManager.GetValue("Catalog.Search.UseFullObjectIndexStoring", false);
                var itemsMap = documents.ToDictionary(doc => doc.Id.ToString(), doc => returnObjectsFromIndex ? doc.GetObjectFieldValue<TItem>() : null);

                var missingObjectIds = itemsMap
                    .Where(kvp => kvp.Value == null)
                    .Select(kvp => kvp.Key)
                    .ToArray();

                if (missingObjectIds.Any())
                {
                    var missingItems = await LoadMissingItems(missingObjectIds, criteria);

                    foreach (var item in missingItems)
                    {
                        itemsMap[item.Id] = item;
                    }
                }

                ReduceSearchResults(itemsMap.Values.Where(v => v != null), criteria);

                // Preserve original sorting order
                result = documents.Select(doc => itemsMap[doc.Id.ToString()]).Where(x => x != null).ToArray();
            }

            return result;
        }
    }
}
