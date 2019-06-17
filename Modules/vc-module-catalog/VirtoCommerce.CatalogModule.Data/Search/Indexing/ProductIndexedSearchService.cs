using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.Search.Indexing
{
    public class ProductIndexedSearchService : CatalogIndexedSearchService<CatalogProduct, ProductIndexedSearchCriteria, ProductIndexedSearchResult>, IProductIndexedSearchService
    {
        private readonly IItemService _itemService;
        private readonly IBlobUrlResolver _blobUrlResolver;
        private readonly IAggregationConverter _aggregationConverter;

        public ProductIndexedSearchService(IEnumerable<ISearchRequestBuilder> searchRequestBuilders, ISearchProvider searchProvider, ISettingsManager settingsManager, IItemService itemService, IBlobUrlResolver blobUrlResolver, IAggregationConverter aggregationConverter)
            : base(searchRequestBuilders, searchProvider, settingsManager)
        {
            _itemService = itemService;
            _blobUrlResolver = blobUrlResolver;
            _aggregationConverter = aggregationConverter;
        }


        protected override async Task<IList<CatalogProduct>> LoadMissingItems(string[] missingItemIds, ProductIndexedSearchCriteria criteria)
        {
            var catalog = criteria.CatalogId;
            var responseGroup = GetResponseGroup(criteria);
            var products = await _itemService.GetByIdsAsync(missingItemIds, responseGroup.ToString(), catalog);
            //var result = products.Select(p => p.ToWebModel(_blobUrlResolver)).ToArray();
            return products;
        }

        protected virtual ItemResponseGroup GetResponseGroup(ProductIndexedSearchCriteria criteria)
        {
            var result = EnumUtility.SafeParseFlags(criteria?.ResponseGroup, ItemResponseGroup.ItemLarge);
            return result;
        }

        protected override void ReduceSearchResults(IEnumerable<CatalogProduct> products, ProductIndexedSearchCriteria criteria)
        {
            var responseGroup = GetResponseGroup(criteria);

            foreach (var product in products)
            {
                product.ReduceDetails(responseGroup.ToString());

                if (!string.IsNullOrEmpty(criteria.LanguageCode))
                {
                    if (!product.Properties.IsNullOrEmpty())
                    {
                        //Return only display names for requested language
                        foreach (var property in product.Properties)
                        {
                            property.DisplayNames = property.DisplayNames?.Where(x => string.IsNullOrEmpty(x.LanguageCode) || x.LanguageCode.EqualsInvariant(criteria.LanguageCode)).ToList();
                            //if (!property.Values.IsNullOrEmpty())
                            //{
                            //    property.Values = property.Values.Where(x => string.IsNullOrEmpty(x.LanguageCode) || x.LanguageCode.EqualsInvariant(criteria.LanguageCode)).ToList();
                            //}
                        }
                    }
                    //Return  only reviews for requested language
                    if (!product.Reviews.IsNullOrEmpty())
                    {
                        product.Reviews = product.Reviews.Where(x => string.IsNullOrEmpty(x.LanguageCode) || x.LanguageCode.EqualsInvariant(criteria.LanguageCode)).ToList();
                    }
                }
            }
        }

        protected override async Task<Aggregation[]> ConvertAggregationsAsync(IList<AggregationResponse> aggregationResponses, ProductIndexedSearchCriteria criteria)
        {
            var aggregationsTasks = _aggregationConverter?.ConvertAggregationsAsync(aggregationResponses, criteria);
            if (aggregationsTasks != null) await Task.WhenAny(aggregationsTasks);
            return aggregationsTasks?.Result;
        }
    }
}
