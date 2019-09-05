using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Search.BrowseFilters;
using VirtoCommerce.CatalogModule.Web.Authorization;
using VirtoCommerce.CatalogModule.Web.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Route("api/catalog/aggregationproperties")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CatalogBrowseFiltersController : Controller
    {
        private const string _attributeType = "Attribute";
        private const string _rangeType = "Range";
        private const string _priceRangeType = "PriceRange";

        private readonly IStoreService _storeService;
        private readonly IPropertyService _propertyService;
        private readonly IPropertySearchService _propertySearchService;
        private readonly IBrowseFilterService _browseFilterService;
        private readonly IPropertyDictionaryItemSearchService _propDictItemsSearchService;

        public CatalogBrowseFiltersController(
            IStoreService storeService
            , IPropertyService propertyService
            , IBrowseFilterService browseFilterService
            , IPropertyDictionaryItemSearchService propDictItemsSearchService
            , IPropertySearchService propertySearchService)
        {
            _storeService = storeService;
            _propertyService = propertyService;
            _browseFilterService = browseFilterService;
            _propDictItemsSearchService = propDictItemsSearchService;
            _propertySearchService = propertySearchService;
        }

        /// <summary>
        /// Get aggregation properties for store
        /// </summary>
        /// <remarks>
        /// Returns all store aggregation properties: selected properties are ordered manually, unselected properties are ordered by name.
        /// </remarks>
        /// <param name="storeId">Store ID</param>
        [HttpGet]
        [Route("{storeId}/properties")]
        [Authorize(ModuleConstants.Security.Permissions.CatalogBrowseFiltersRead)]
        public async Task<ActionResult<AggregationProperty[]>> GetAggregationProperties(string storeId)
        {
            var store = await _storeService.GetByIdAsync(storeId, StoreResponseGroup.StoreInfo.ToString());
            if (store == null)
            {
                return NoContent();
            }
            
            var allProperties = await GetAllPropertiesAsync(store.Catalog, store.Currencies);
            var selectedProperties = await GetSelectedPropertiesAsync(storeId);

            // Remove duplicates and keep selected properties order
            var result = selectedProperties.Concat(allProperties)
                .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToArray();

            return Ok(result);
        }

        /// <summary>
        /// Set aggregation properties for store
        /// </summary>
        /// <param name="storeId">Store ID</param>
        /// <param name="browseFilterProperties"></param>
        [HttpPut]
        [Route("{storeId}/properties")]
        [Authorize(ModuleConstants.Security.Permissions.CatalogBrowseFiltersUpdate)]
        public async Task<ActionResult> SetAggregationProperties(string storeId, [FromBody]AggregationProperty[] browseFilterProperties)
        {
            var store = await _storeService.GetByIdAsync(storeId, StoreResponseGroup.StoreInfo.ToString());
            if (store == null)
            {
                return NoContent();
            }       
            // Filter names must be unique
            // Keep the selected properties order.
            var filters = browseFilterProperties
                .Where(p => p.IsSelected)
                .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                .Select((g, i) => ConvertToFilter(g.First(), i))
                .Where(f => f != null)
                .ToArray();

            await _browseFilterService.SaveStoreAggregationsAsync(storeId, filters);

            return NoContent();
        }

        [HttpGet]
        [Route("{storeId}/properties/{propertyName}/values")]
        public async Task<ActionResult<string[]>> GetPropertyValues(string storeId, string propertyName)
        {
            var result = Array.Empty<string>();
            var store = await _storeService.GetByIdAsync(storeId, StoreResponseGroup.StoreInfo.ToString());
            if (store != null)
            {
                var catalogPropertiesSearchResult = await _propertySearchService.SearchPropertiesAsync(new PropertySearchCriteria { PropertyNames = new[] { propertyName }, CatalogId = store.Catalog, Take = 1 });
                var property = catalogPropertiesSearchResult.Results.FirstOrDefault(p => p.Name.EqualsInvariant(propertyName) && p.Dictionary);
                if (property != null)
                {                  
                    var searchResult = await _propDictItemsSearchService.SearchAsync(new PropertyDictionaryItemSearchCriteria { PropertyIds = new[] { property.Id }, Take = int.MaxValue });
                    result = searchResult.Results.Select(x => x.Alias).Distinct().ToArray();
                }
            }
            return Ok(result);
        }


        private async Task<IList<AggregationProperty>> GetAllPropertiesAsync(string catalogId, IEnumerable<string> currencies)
        {
            var result = (await _propertySearchService.SearchPropertiesAsync(new PropertySearchCriteria { CatalogId = catalogId, Take = int.MaxValue })).Results
                            .Select(p => new AggregationProperty { Type = _attributeType, Name = p.Name })
                            .ToList();

            result.AddRange(currencies.Select(c => new AggregationProperty { Type = _priceRangeType, Name = $"Price {c}", Currency = c }));

            return result;
        }

        private async Task<IList<AggregationProperty>> GetSelectedPropertiesAsync(string storeId)
        {
            var result = new List<AggregationProperty>();

            var allFilters = await _browseFilterService.GetStoreAggregationsAsync(storeId);
            if (allFilters != null)
            {
                AggregationProperty property = null;

                foreach (var filter in allFilters)
                {
                    var attributeFilter = filter as AttributeFilter;
                    var rangeFilter = filter as RangeFilter;
                    var priceRangeFilter = filter as PriceRangeFilter;

                    if (attributeFilter != null)
                    {
                        property = new AggregationProperty
                        {
                            IsSelected = true,
                            Type = _attributeType,
                            Name = attributeFilter.Key,
                            Values = attributeFilter.Values?.Select(v => v.Id).OrderBy(v => v, StringComparer.OrdinalIgnoreCase).ToArray(),
                            Size = attributeFilter.FacetSize,
                        };
                    }
                    else if (rangeFilter != null)
                    {
                        property = new AggregationProperty
                        {
                            IsSelected = true,
                            Type = _rangeType,
                            Name = rangeFilter.Key,
                            Values = GetRangeBounds(rangeFilter.Values),
                        };
                    }
                    else if (priceRangeFilter != null)
                    {
                        property = new AggregationProperty
                        {
                            IsSelected = true,
                            Type = _priceRangeType,
                            Name = $"Price {priceRangeFilter.Currency}",
                            Values = GetRangeBounds(priceRangeFilter.Values),
                            Currency = priceRangeFilter.Currency,
                        };
                    }

                    if (property != null)
                    {
                        result.Add(property);
                    }
                }
            }

            return result;
        }

        private static IList<string> GetRangeBounds(IEnumerable<RangeFilterValue> values)
        {
            return SortStringsAsNumbers(values?.SelectMany(v => new[] { v.Lower, v.Upper }))?.ToArray();
        }

        private static IBrowseFilter ConvertToFilter(AggregationProperty property, int order)
        {
            IBrowseFilter result = null;

            switch (property.Type)
            {
                case _attributeType:
                    result = new AttributeFilter
                    {
                        Order = order,
                        Key = property.Name,
                        FacetSize = property.Size,
                        Values = property.Values?.OrderBy(v => v, StringComparer.OrdinalIgnoreCase).Select(v => new AttributeFilterValue { Id = v }).ToArray(),
                    };
                    break;
                case _rangeType:
                    result = new RangeFilter
                    {
                        Order = order,
                        Key = property.Name,
                        Values = GetRangeFilterValues(property.Values),
                    };
                    break;
                case _priceRangeType:
                    result = new PriceRangeFilter
                    {
                        Order = order,
                        Currency = property.Currency,
                        Values = GetRangeFilterValues(property.Values),
                    };
                    break;
            }

            return result;
        }

        private static RangeFilterValue[] GetRangeFilterValues(IList<string> bounds)
        {
            var result = new List<RangeFilterValue>();

            if (bounds?.Any() == true)
            {
                var sortedBounds = SortStringsAsNumbers(bounds).ToList();
                sortedBounds.Add(null);

                string previousBound = null;

                foreach (var bound in sortedBounds)
                {
                    var value = new RangeFilterValue
                    {
                        Id = previousBound == null ? $"under-{bound}" : bound == null ? $"over-{previousBound}" : $"{previousBound}-{bound}",
                        Lower = previousBound,
                        Upper = bound,
                        IncludeLower = true,
                        IncludeUpper = false,
                    };

                    result.Add(value);
                    previousBound = bound;
                }
            }

            return result.Any() ? result.ToArray() : null;
        }

        private static IEnumerable<string> SortStringsAsNumbers(IEnumerable<string> strings)
        {
            return strings
                ?.Where(b => !string.IsNullOrEmpty(b))
                .Select(b => decimal.Parse(b, NumberStyles.Float, CultureInfo.InvariantCulture))
                .OrderBy(b => b)
                .Distinct()
                .Select(b => b.ToString(CultureInfo.InvariantCulture));
        }
    }
}
