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
    public class ProductSearchService : CatalogSearchService<CatalogProduct, ProductSearchCriteria, ProductSearchResult>, IProductSearchService
    {
        private readonly IItemService _itemService;
        private readonly IBlobUrlResolver _blobUrlResolver;
        private readonly IAggregationConverter _aggregationConverter;

        public ProductSearchService(ISearchRequestBuilder[] searchRequestBuilders, ISearchProvider searchProvider, ISettingsManager settingsManager, IItemService itemService, IBlobUrlResolver blobUrlResolver, IAggregationConverter aggregationConverter)
            : base(searchRequestBuilders, searchProvider, settingsManager)
        {
            _itemService = itemService;
            _blobUrlResolver = blobUrlResolver;
            _aggregationConverter = aggregationConverter;
        }


        protected override async Task<IList<CatalogProduct>> LoadMissingItems(string[] missingItemIds, ProductSearchCriteria criteria)
        {
            var catalog = criteria.CatalogId;
            var responseGroup = GetResponseGroup(criteria);
            var products = await _itemService.GetByIdsAsync(missingItemIds, responseGroup, catalog);
            //var result = products.Select(p => p.ToWebModel(_blobUrlResolver)).ToArray();
            return products;
        }

        protected virtual ItemResponseGroup GetResponseGroup(ProductSearchCriteria criteria)
        {
            var result = EnumUtility.SafeParse(criteria?.ResponseGroup, ItemResponseGroup.ItemLarge);
            return result;
        }

        protected override void ReduceSearchResults(IEnumerable<CatalogProduct> products, ProductSearchCriteria criteria)
        {
            var responseGroup = GetResponseGroup(criteria);

            foreach (var product in products)
            {
                if (!responseGroup.HasFlag(ItemResponseGroup.ItemAssets))
                {
                    product.Assets = null;
                }

                if (!responseGroup.HasFlag(ItemResponseGroup.ItemProperties))
                {
                    product.Properties = null;
                }
                else if (!string.IsNullOrEmpty(criteria.LanguageCode))
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
                }
                if (!responseGroup.HasFlag(ItemResponseGroup.ItemAssociations))
                {
                    product.Associations = null;
                }

                if (!responseGroup.HasFlag(ItemResponseGroup.ItemEditorialReviews))
                {
                    product.Reviews = null;
                }
                else if (!string.IsNullOrEmpty(criteria.LanguageCode))
                {
                    //Return  only reviews for requested language
                    if (!product.Reviews.IsNullOrEmpty())
                    {
                        product.Reviews = product.Reviews.Where(x => string.IsNullOrEmpty(x.LanguageCode) || x.LanguageCode.EqualsInvariant(criteria.LanguageCode)).ToList();
                    }
                }

                if (!responseGroup.HasFlag(ItemResponseGroup.Links))
                {
                    product.Links = null;
                }

                if (!responseGroup.HasFlag(ItemResponseGroup.Outlines))
                {
                    product.Outlines = null;
                }

                if (!responseGroup.HasFlag(ItemResponseGroup.Seo))
                {
                    product.SeoInfos = null;
                }

                if (!responseGroup.HasFlag(ItemResponseGroup.Variations))
                {
                    product.Variations = null;
                }
            }
        }

        protected override Aggregation[] ConvertAggregations(IList<AggregationResponse> aggregationResponses, ProductSearchCriteria criteria)
        {
            return _aggregationConverter?.ConvertAggregations(aggregationResponses, criteria);
        }
    }
}
