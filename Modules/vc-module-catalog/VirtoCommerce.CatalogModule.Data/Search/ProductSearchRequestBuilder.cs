using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Catalog.Model.Search;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public class ProductSearchRequestBuilder : ISearchRequestBuilder
    {
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly ITermFilterBuilder _termFilterBuilder;
        private readonly IAggregationConverter _aggregationConverter;

        public ProductSearchRequestBuilder(ISearchPhraseParser searchPhraseParser, ITermFilterBuilder termFilterBuilder, IAggregationConverter aggregationConverter)
        {
            _searchPhraseParser = searchPhraseParser;
            _termFilterBuilder = termFilterBuilder;
            _aggregationConverter = aggregationConverter;
        }

        public virtual string DocumentType { get; } = KnownDocumentTypes.Product;

        public virtual SearchRequest BuildRequest(SearchCriteriaBase criteria)
        {
            SearchRequest request = null;

            var productSearchCriteria = criteria as ProductSearchCriteria;
            if (productSearchCriteria != null)
            {
                // Getting filters modifies search phrase
                var allFilters = GetAllFilters(productSearchCriteria);

                request = new SearchRequest
                {
                    SearchKeywords = productSearchCriteria.SearchPhrase,
                    SearchFields = new[] { "__content" },
                    Filter = GetFilters(allFilters).And(),
                    Sorting = GetSorting(productSearchCriteria),
                    Skip = criteria.Skip,
                    Take = criteria.Take,
                    Aggregations = _aggregationConverter?.GetAggregationRequests(productSearchCriteria, allFilters),
                    IsFuzzySearch = productSearchCriteria.IsFuzzySearch,
                    RawQuery = productSearchCriteria.RawQuery
            };
            }

            return request;
        }


        protected virtual IList<SortingField> GetSorting(ProductSearchCriteria criteria)
        {
            var result = new List<SortingField>();

            var priorityFields = criteria.GetPriorityFields();

            foreach (SortInfo sortInfo in criteria.SortInfos)
            {
                var sortingField = new SortingField();
                if (sortInfo is GeoSortInfo geoSortInfo)
                {
                    sortingField = new GeoDistanceSortingField
                    {
                        Location = geoSortInfo.GeoPoint
                    };
                }
                sortingField.FieldName = sortInfo.SortColumn.ToLowerInvariant();
                sortingField.IsDescending = sortInfo.SortDirection == SortDirection.Descending;

                switch (sortingField.FieldName)
                {
                    case "price":
                        if (criteria.Pricelists != null)
                        {
                            result.AddRange(
                                criteria.Pricelists.Select(priceList => new SortingField($"price_{criteria.Currency}_{priceList}".ToLowerInvariant(), sortingField.IsDescending)));
                        }
                        break;
                    case "priority":
                        result.AddRange(priorityFields.Select(priorityField => new SortingField(priorityField, sortingField.IsDescending)));
                        break;
                    case "name":
                    case "title":
                        result.Add(new SortingField("name", sortingField.IsDescending));
                        break;
                    default:
                        result.Add(sortingField);
                        break;
                }
            }

            if (!result.Any())
            {
                result.AddRange(priorityFields.Select(priorityField => new SortingField(priorityField, true)));
                result.Add(new SortingField("__sort"));
            }

            return result;
        }

        protected virtual IList<IFilter> GetFilters(FiltersContainer allFilters)
        {
            return allFilters.GetFiltersExceptSpecified(null);
        }

        protected virtual FiltersContainer GetAllFilters(ProductSearchCriteria criteria)
        {
            var permanentFilters = GetPermanentFilters(criteria);
            var termFilters = _termFilterBuilder.GetTermFilters(criteria);

            var result = new FiltersContainer
            {
                PermanentFilters = permanentFilters.Concat(termFilters.PermanentFilters).ToList(),
                RemovableFilters = termFilters.RemovableFilters,
            };

            return result;
        }

        protected virtual IList<IFilter> GetPermanentFilters(ProductSearchCriteria criteria)
        {
            var result = new List<IFilter>();

            if (!string.IsNullOrEmpty(criteria.SearchPhrase))
            {
                var parseResult = _searchPhraseParser.Parse(criteria.SearchPhrase);
                criteria.SearchPhrase = parseResult.SearchPhrase;
                result.AddRange(parseResult.Filters);
            }

            if (criteria.ObjectIds != null)
            {
                result.Add(new IdsFilter { Values = criteria.ObjectIds });
            }

            if (!string.IsNullOrEmpty(criteria.CatalogId))
            {
                result.Add(FiltersHelper.CreateTermFilter("catalog", criteria.CatalogId.ToLowerInvariant()));
            }

            result.Add(FiltersHelper.CreateOutlineFilter(criteria));

            result.Add(FiltersHelper.CreateDateRangeFilter("startdate", criteria.StartDateFrom, criteria.StartDate, false, true));

            if (criteria.EndDate != null)
            {
                result.Add(FiltersHelper.CreateDateRangeFilter("enddate", criteria.EndDate, null, false, false));
            }

            if (!criteria.ClassTypes.IsNullOrEmpty())
            {
                result.Add(FiltersHelper.CreateTermFilter("__type", criteria.ClassTypes));
            }

            if (!criteria.WithHidden)
            {
                result.Add(FiltersHelper.CreateTermFilter("status", "visible"));
            }

            if (criteria.PriceRange != null)
            {
                var range = criteria.PriceRange;
                result.Add(FiltersHelper.CreatePriceRangeFilter(criteria.Currency, null, range.Lower, range.Upper, range.IncludeLower, range.IncludeUpper));
            }

            if (criteria.GeoDistanceFilter != null)
            {
                result.Add(criteria.GeoDistanceFilter);
            }

            return result;
        }
    }
}
