using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Data.Search.BrowseFilters;
using VirtoCommerce.CatalogModule.Web.Model;
using VirtoCommerce.Domain.Catalog.Model.Search;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Common;
using RangeFilter = VirtoCommerce.CatalogModule.Data.Search.BrowseFilters.RangeFilter;
using RangeFilterValue = VirtoCommerce.CatalogModule.Data.Search.BrowseFilters.RangeFilterValue;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public class AggregationConverter : IAggregationConverter
    {
        private readonly IBrowseFilterService _browseFilterService;
        private readonly IPropertyService _propertyService;
        private readonly IProperyDictionaryItemSearchService _propDictItemsSearchService;

        public AggregationConverter(IBrowseFilterService browseFilterService, IPropertyService propertyService, IProperyDictionaryItemSearchService propDictItemsSearchService)
        {
            _browseFilterService = browseFilterService;
            _propertyService = propertyService;
            _propDictItemsSearchService = propDictItemsSearchService;
        }

        #region Request converter

        public IList<AggregationRequest> GetAggregationRequests(ProductSearchCriteria criteria, FiltersContainer allFilters)
        {
            var result = new List<AggregationRequest>();

            var browseFilters = _browseFilterService.GetBrowseFilters(criteria);
            if (browseFilters != null)
            {
                foreach (var filter in browseFilters)
                {
                    var existingFilters = allFilters.GetFiltersExceptSpecified(filter.Key);

                    var attributeFilter = filter as AttributeFilter;
                    var priceRangeFilter = filter as PriceRangeFilter;
                    var rangeFilter = filter as RangeFilter;

                    AggregationRequest aggregationRequest = null;
                    IList<AggregationRequest> aggregationRequests = null;

                    if (attributeFilter != null)
                    {
                        aggregationRequest = GetAttributeFilterAggregationRequest(attributeFilter, existingFilters);
                    }
                    else if (rangeFilter != null)
                    {
                        aggregationRequests = GetRangeFilterAggregationRequests(rangeFilter, existingFilters);
                    }
                    else if (priceRangeFilter != null && priceRangeFilter.Currency.EqualsInvariant(criteria.Currency))
                    {
                        aggregationRequests = GetPriceRangeFilterAggregationRequests(priceRangeFilter, criteria, existingFilters);
                    }

                    if (aggregationRequest != null)
                    {
                        result.Add(aggregationRequest);
                    }

                    if (aggregationRequests != null)
                    {
                        result.AddRange(aggregationRequests.Where(f => f != null));
                    }
                }
            }

            return result;
        }


        protected virtual AggregationRequest GetAttributeFilterAggregationRequest(AttributeFilter attributeFilter, IEnumerable<IFilter> existingFilters)
        {
            return new TermAggregationRequest
            {
                FieldName = attributeFilter.Key,
                Values = !attributeFilter.Values.IsNullOrEmpty() ? attributeFilter.Values.Select(v => v.Id).ToArray() : null,
                Filter = existingFilters.And(),
                Size = attributeFilter.FacetSize,
            };
        }

        protected virtual IList<AggregationRequest> GetRangeFilterAggregationRequests(RangeFilter rangeFilter, IList<IFilter> existingFilters)
        {
            var result = rangeFilter.Values?.Select(v => GetRangeFilterValueAggregationRequest(rangeFilter.Key, v, existingFilters)).ToList();
            return result;
        }

        protected virtual AggregationRequest GetRangeFilterValueAggregationRequest(string fieldName, RangeFilterValue value, IEnumerable<IFilter> existingFilters)
        {
            var valueFilter = FiltersHelper.CreateRangeFilter(fieldName, value.Lower, value.Upper, value.IncludeLower, value.IncludeUpper);

            var result = new TermAggregationRequest
            {
                Id = $"{fieldName}-{value.Id}",
                Filter = existingFilters.And(valueFilter)
            };

            return result;
        }

        protected virtual IList<AggregationRequest> GetPriceRangeFilterAggregationRequests(PriceRangeFilter priceRangeFilter, ProductSearchCriteria criteria, IList<IFilter> existingFilters)
        {
            var result = priceRangeFilter.Values?.Select(v => GetPriceRangeFilterValueAggregationRequest(priceRangeFilter, v, existingFilters, criteria.Pricelists)).ToList();
            return result;
        }

        protected virtual AggregationRequest GetPriceRangeFilterValueAggregationRequest(PriceRangeFilter priceRangeFilter, RangeFilterValue value, IEnumerable<IFilter> existingFilters, IList<string> pricelists)
        {
            var valueFilter = FiltersHelper.CreatePriceRangeFilter(priceRangeFilter.Currency, pricelists, value.Lower, value.Upper, value.IncludeLower, value.IncludeUpper);

            var result = new TermAggregationRequest
            {
                Id = $"{priceRangeFilter.Key}-{value.Id}",
                Filter = existingFilters.And(valueFilter)
            };

            return result;
        }

        #endregion

        #region Response converter

        public virtual Aggregation[] ConvertAggregations(IList<AggregationResponse> aggregationResponses, ProductSearchCriteria criteria)
        {
            var result = new List<Aggregation>();

            var browseFilters = _browseFilterService.GetBrowseFilters(criteria);
            if (browseFilters != null && aggregationResponses?.Any() == true)
            {
                foreach (var filter in browseFilters)
                {
                    Aggregation aggregation = null;

                    var attributeFilter = filter as AttributeFilter;
                    var rangeFilter = filter as RangeFilter;
                    var priceRangeFilter = filter as PriceRangeFilter;

                    if (attributeFilter != null)
                    {
                        aggregation = GetAttributeAggregation(attributeFilter, aggregationResponses);
                    }
                    else if (rangeFilter != null)
                    {
                        aggregation = GetRangeAggregation(rangeFilter, aggregationResponses);
                    }
                    else if (priceRangeFilter != null)
                    {
                        aggregation = GetPriceRangeAggregation(priceRangeFilter, aggregationResponses);
                    }

                    if (aggregation?.Items?.Any() == true)
                    {
                        result.Add(aggregation);
                    }
                }
            }

            // Add localized labels for names and values
            if (result.Any())
            {
                AddLabels(result, criteria.CatalogId);
            }

            return result.ToArray();
        }

        protected virtual Aggregation GetAttributeAggregation(AttributeFilter attributeFilter, IList<AggregationResponse> aggregationResponses)
        {
            Aggregation result = null;

            var fieldName = attributeFilter.Key;
            var aggregationResponse = aggregationResponses.FirstOrDefault(a => a.Id.EqualsInvariant(fieldName));

            if (aggregationResponse != null)
            {
                IList<AggregationResponseValue> aggregationResponseValues;

                if (attributeFilter.Values.IsNullOrEmpty())
                {
                    // Return all values
                    aggregationResponseValues = aggregationResponse.Values;
                }
                else
                {
                    // Return predefined values
                    aggregationResponseValues = attributeFilter.Values
                        .GroupBy(v => v.Id, StringComparer.OrdinalIgnoreCase)
                        .Select(g => aggregationResponse.Values.FirstOrDefault(v => v.Id.EqualsInvariant(g.Key)))
                        .Where(v => v != null)
                        .ToArray();
                }

                if (aggregationResponseValues.Any())
                {
                    result = new Aggregation
                    {
                        AggregationType = "attr",
                        Field = fieldName,
                        Items = GetAttributeAggregationItems(aggregationResponseValues),
                    };
                }
            }

            return result;
        }

        protected virtual Aggregation GetRangeAggregation(RangeFilter rangeFilter, IList<AggregationResponse> aggregationResponses)
        {
            var result = new Aggregation
            {
                AggregationType = "range",
                Field = rangeFilter.Key,
                Items = GetRangeAggregationItems(rangeFilter.Key, rangeFilter.Values, aggregationResponses),
            };

            return result;
        }

        protected virtual Aggregation GetPriceRangeAggregation(PriceRangeFilter priceRangeFilter, IList<AggregationResponse> aggregationResponses)
        {
            var result = new Aggregation
            {
                AggregationType = "pricerange",
                Field = priceRangeFilter.Key,
                Items = GetRangeAggregationItems(priceRangeFilter.Key, priceRangeFilter.Values, aggregationResponses),
            };


            return result;
        }

        protected virtual IList<AggregationItem> GetAttributeAggregationItems(IList<AggregationResponseValue> aggregationResponseValues)
        {
            var result = aggregationResponseValues
                .Select(v =>
                    new AggregationItem
                    {
                        Value = v.Id,
                        Count = (int)v.Count,
                    })
                .ToList();

            return result;
        }

        protected virtual IList<AggregationItem> GetRangeAggregationItems(string aggregationId, IList<RangeFilterValue> values, IList<AggregationResponse> aggregationResponses)
        {
            var result = new List<AggregationItem>();

            if (values != null)
            {
                foreach (var group in values.GroupBy(v => v.Id, StringComparer.OrdinalIgnoreCase))
                {
                    var valueId = group.Key;
                    var aggregationValueId = $"{aggregationId}-{valueId}";
                    var aggregationResponse = aggregationResponses.FirstOrDefault(v => v.Id.EqualsInvariant(aggregationValueId));

                    if (aggregationResponse?.Values?.Any() == true)
                    {
                        var value = aggregationResponse.Values.First();
                        var rangeValue = group.First();

                        var aggregationItem = new AggregationItem
                        {
                            Value = valueId,
                            Count = (int)value.Count,
                            RequestedLowerBound = !string.IsNullOrEmpty(rangeValue.Lower) ? rangeValue.Lower : null,
                            RequestedUpperBound = !string.IsNullOrEmpty(rangeValue.Upper) ? rangeValue.Upper : null,
                        };

                        result.Add(aggregationItem);
                    }
                }
            }

            return result;
        }

        protected virtual void AddLabels(IList<Aggregation> aggregations, string catalogId)
        {
            var allProperties = _propertyService.GetAllCatalogProperties(catalogId);

            foreach (var aggregation in aggregations)
            {
                // There can be many properties with the same name
                var properties = allProperties.Where(p => p.Name.EqualsInvariant(aggregation.Field)).ToArray();

                if (properties.Any())
                {
                    var allPropertyLabels = properties.SelectMany(p => p.DisplayNames)
                                                      .Select(n => new AggregationLabel { Language = n.LanguageCode, Label = n.Name })
                                                      .ToArray();

                    aggregation.Labels = GetFirstLabelForEachLanguage(allPropertyLabels);

                    var allDictItemsMap = _propDictItemsSearchService.Search(new PropertyDictionaryItemSearchCriteria { PropertyIds = properties.Select(x => x.Id).ToArray(), Take = int.MaxValue })
                                                                     .Results.GroupBy(x => x.Alias)
                                                                     .ToDictionary(x => x.Key, x => x.SelectMany(dictItem => dictItem.LocalizedValues)
                                                                                                     .Select(localizedValue => new AggregationLabel { Language = localizedValue.LanguageCode, Label = localizedValue.Value }));

                    foreach (var aggregationItem in aggregation.Items)
                    {
                        var alias = aggregationItem.Value?.ToString();
                        if (!string.IsNullOrEmpty(alias))
                        {
                            if (allDictItemsMap.TryGetValue(alias, out var labels))
                            {
                                aggregationItem.Labels = GetFirstLabelForEachLanguage(labels);
                            }
                        }
                    }
                }
            }
        }

        private static IList<AggregationLabel> GetFirstLabelForEachLanguage(IEnumerable<AggregationLabel> labels)
        {
            var result = labels
                .Where(x => !string.IsNullOrEmpty(x.Language) && !string.IsNullOrEmpty(x.Label))
                .GroupBy(x => x.Language, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.FirstOrDefault())
                .OrderBy(x => x?.Language)
                .ThenBy(x => x.Label)
                .ToArray();

            return result.Any() ? result : null;
        }

        #endregion
    }
}
