using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Data.Search.BrowseFilters;
using VirtoCommerce.Domain.Catalog.Model.Search;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Common;
using RangeFilter = VirtoCommerce.Domain.Search.RangeFilter;
using RangeFilterValue = VirtoCommerce.Domain.Search.RangeFilterValue;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public class TermFilterBuilder : ITermFilterBuilder
    {
        private readonly IBrowseFilterService _browseFilterService;

        public TermFilterBuilder(IBrowseFilterService browseFilterService)
        {
            _browseFilterService = browseFilterService;
        }

        public virtual FiltersContainer GetTermFilters(ProductSearchCriteria criteria)
        {
            var result = new FiltersContainer();

            var terms = criteria.GetTerms();
            if (terms.Any())
            {
                var browseFilters = _browseFilterService.GetBrowseFilters(criteria);

                var filtersAndValues = browseFilters
                    ?.Select(f => new { Filter = f, Values = f.GetValues() })
                    .ToList();

                foreach (var term in terms)
                {
                    var browseFilter = browseFilters?.SingleOrDefault(x => x.Key.EqualsInvariant(term.Key));

                    // Handle special filter term with a key = "tags", it contains just values and we need to determine which filter to use
                    if (browseFilter == null && term.Key == "tags")
                    {
                        foreach (var termValue in term.Values)
                        {
                            // Try to find filter by value
                            var filterAndValues = filtersAndValues?.FirstOrDefault(x => x.Values?.Any(v => v.Id.Equals(termValue)) == true);
                            if (filterAndValues != null)
                            {
                                var filter = ConvertBrowseFilter(filterAndValues.Filter, term.Values, criteria);
                                result.PermanentFilters.Add(filter);
                            }
                            else
                            {
                                // Unknown term values should produce empty result
                                result.PermanentFilters.Add(new IdsFilter { Values = new[] { string.Empty } });
                            }
                        }
                    }
                    else if (browseFilter != null) // Predefined filter
                    {
                        var filter = ConvertBrowseFilter(browseFilter, term.Values, criteria);
                        result.RemovableFilters.Add(new KeyValuePair<string, IFilter>(browseFilter.Key, filter));
                    }
                    else // Custom term
                    {
                        var filter = FiltersHelper.CreateTermFilter(term.Key, term.Values);
                        result.PermanentFilters.Add(filter);
                    }
                }
            }

            return result;
        }


        protected virtual IFilter ConvertBrowseFilter(IBrowseFilter filter, IList<string> valueIds, ProductSearchCriteria criteria)
        {
            IFilter result = null;

            if (filter != null && valueIds != null)
            {
                var attributeFilter = filter as AttributeFilter;
                var rangeFilter = filter as BrowseFilters.RangeFilter;
                var priceRangeFilter = filter as PriceRangeFilter;

                if (attributeFilter != null)
                {
                    result = ConvertAttributeFilter(attributeFilter, valueIds);
                }
                else if (rangeFilter != null)
                {
                    result = ConvertRangeFilter(rangeFilter, valueIds);
                }
                else if (priceRangeFilter != null)
                {
                    result = ConvertPriceRangeFilter(priceRangeFilter, valueIds, criteria);
                }
            }

            return result;
        }

        protected virtual IFilter ConvertAttributeFilter(AttributeFilter attributeFilter, IList<string> valueIds)
        {
            var knownValues = attributeFilter.Values
                ?.Where(v => valueIds.Contains(v.Id, StringComparer.OrdinalIgnoreCase))
                .Select(v => v.Id)
                .ToArray();

            var result = new TermFilter
            {
                FieldName = attributeFilter.Key,
                Values = knownValues != null && knownValues.Any() ? knownValues : valueIds,
            };

            return result;
        }

        protected virtual IFilter ConvertPriceRangeFilter(PriceRangeFilter priceRangeFilter, IList<string> valueIds, ProductSearchCriteria criteria)
        {
            IFilter result = null;

            if (string.IsNullOrEmpty(criteria.Currency) || priceRangeFilter.Currency.EqualsInvariant(criteria.Currency))
            {
                var knownValues = priceRangeFilter.Values
                    ?.Where(v => valueIds.Contains(v.Id, StringComparer.OrdinalIgnoreCase))
                    .ToArray();

                if (knownValues != null && knownValues.Any())
                {
                    var filters = knownValues
                        .Select(v => FiltersHelper.CreatePriceRangeFilter(priceRangeFilter.Currency, criteria.Pricelists, v.Lower, v.Upper, v.IncludeLower, v.IncludeUpper))
                        .Where(f => f != null)
                        .ToList();

                    result = filters.Or();
                }
                else
                {
                    // Unknown term values should produce empty result
                    result = new IdsFilter { Values = new[] { string.Empty } };
                }
            }

            return result;
        }

        protected virtual IFilter ConvertRangeFilter(BrowseFilters.RangeFilter rangeFilter, IList<string> valueIds)
        {
            IFilter result;

            var knownValues = rangeFilter.Values
                ?.Where(v => valueIds.Contains(v.Id, StringComparer.OrdinalIgnoreCase))
                .ToArray();

            if (knownValues != null && knownValues.Any())
            {
                result = new RangeFilter
                {
                    FieldName = rangeFilter.Key,
                    Values = knownValues.Select(ConvertRangeFilterValue).ToArray(),
                };
            }
            else
            {
                // Unknown term values should produce empty result
                result = new IdsFilter { Values = new[] { string.Empty } };
            }

            return result;
        }

        protected virtual RangeFilterValue ConvertRangeFilterValue(BrowseFilters.RangeFilterValue rangeFilterValue)
        {
            return new RangeFilterValue
            {
                Lower = rangeFilterValue.Lower,
                Upper = rangeFilterValue.Upper,
                IncludeLower = rangeFilterValue.IncludeLower,
                IncludeUpper = rangeFilterValue.IncludeUpper,
            };
        }
    }
}
