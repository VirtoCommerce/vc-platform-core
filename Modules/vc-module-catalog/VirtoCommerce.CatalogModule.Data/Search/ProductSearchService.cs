using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Web.Converters;
using VirtoCommerce.CatalogModule.Web.Model;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Model.Search;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using Aggregation = VirtoCommerce.CatalogModule.Web.Model.Aggregation;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public class ProductSearchService : CatalogSearchService<Product, ProductSearchCriteria, ProductSearchResult>, IProductSearchService
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


        protected override IList<Product> LoadMissingItems(string[] missingItemIds, ProductSearchCriteria criteria)
        {
            var catalog = criteria.CatalogId;
            var responseGroup = GetResponseGroup(criteria);           
            var products = _itemService.GetByIds(missingItemIds, responseGroup, catalog);         
            var result = products.Select(p => p.ToWebModel(_blobUrlResolver)).ToArray();         
            return result;
        }

        protected virtual ItemResponseGroup GetResponseGroup(ProductSearchCriteria criteria)
        {
            var result = EnumUtility.SafeParse(criteria?.ResponseGroup, ItemResponseGroup.ItemLarge);
            return result;
        }

        protected override void ReduceSearchResults(IEnumerable<Product> products, ProductSearchCriteria criteria)
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
                else if(!string.IsNullOrEmpty(criteria.LanguageCode))
                {
                    if(!product.Properties.IsNullOrEmpty())
                    {
                        //Return only display names for requested language
                        foreach (var property in product.Properties)
                        {
                            property.DisplayNames = property.DisplayNames?.Where(x => string.IsNullOrEmpty(x.LanguageCode) || x.LanguageCode.EqualsInvariant(criteria.LanguageCode)).ToList();
                            if(!property.Values.IsNullOrEmpty())
                            {
                                property.Values = property.Values.Where(x => string.IsNullOrEmpty(x.LanguageCode) || x.LanguageCode.EqualsInvariant(criteria.LanguageCode)).ToList();
                            }
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
                    if(!product.Reviews.IsNullOrEmpty())
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
