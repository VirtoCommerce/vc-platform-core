using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Azure.Search.Models;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.AzureSearchModule.Data
{
    public class AzureSearchRequestBuilder
    {
        public static IList<AzureSearchRequest> BuildRequest(SearchRequest request, string indexName, string documentType, IList<Field> availableFields)
        {
            var result = new List<AzureSearchRequest>();

            // Create additional requests for each aggregation with fillter which differs from main request filter or with empty field name.

            var searchText = GetSearchText(request);
            var primaryFilter = GetFilters(request, availableFields);
            var sorting = GetSorting(request, availableFields);

            var primaryFacets = new List<string>();
            var facetRequests = GetFacets(request, availableFields);

            foreach (var filterGroup in facetRequests.GroupBy(f => f.Filter))
            {
                if (filterGroup.Key == primaryFilter)
                {
                    primaryFacets.AddRange(filterGroup.Select(f => f.Facet));
                }
                else
                {
                    foreach (var fieldGroup in filterGroup.GroupBy(f => f.FieldName))
                    {
                        if (string.IsNullOrEmpty(fieldGroup.Key))
                        {
                            foreach (var facetRequest in fieldGroup)
                            {
                                result.Add(CreateRequest(searchText, facetRequest.Id, facetRequest.Filter, null, null, 0, 0));
                            }
                        }
                        else
                        {
                            result.Add(CreateRequest(searchText, null, filterGroup.Key, filterGroup.Select(f => f.Facet).ToArray(), null, 0, 0));
                        }
                    }
                }
            }
            var primaryRequest = CreateRequest(searchText, null, primaryFilter, primaryFacets, sorting, request.Skip, request.Take);
            if (!string.IsNullOrEmpty(request.RawQuery))
            {
                primaryRequest = CreateRawQueryRequest(request, sorting, request.Skip, request.Take);
            }
            result.Insert(0, primaryRequest);
            return result;
        }

        private static AzureSearchRequest CreateRawQueryRequest(SearchRequest request, IList<string> sorting, int skip, int take)
        {
            var searchParameters = new SearchParameters
            {
                Filter = request.RawQuery,
                OrderBy = sorting,
                Skip = skip,
                Top = take
            };
            return new AzureSearchRequest { SearchParameters = searchParameters };
        }
        private static AzureSearchRequest CreateRequest(string searchText, string aggregationId, string filter, IList<string> facets, IList<string> orderBy, int skip, int top)
        {
            return new AzureSearchRequest
            {
                SearchText = searchText,
                AggregationId = aggregationId,
                SearchParameters = new SearchParameters
                {
                    QueryType = QueryType.Simple,
                    SearchMode = SearchMode.All,
                    IncludeTotalResultCount = true,
                    Filter = filter,
                    Facets = facets,
                    OrderBy = orderBy,
                    Skip = skip,
                    Top = top,
                }
            };
        }

        private static string GetSearchText(SearchRequest request)
        {
            return request?.SearchKeywords;
        }

        private static IList<string> GetSorting(SearchRequest request, IList<Field> availableFields)
        {
            IList<string> result = null;

            if (request.Sorting != null)
            {
                var fields = request.Sorting
                    .Select(f => GetSortingField(f, availableFields))
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                if (fields.Any())
                {
                    result = fields;
                }
            }

            return result;
        }

        private static string GetSortingField(SortingField sortingField, IList<Field> availableFields)
        {
            string result = null;

            var availableField = availableFields.Get(sortingField.FieldName);
            if (availableField != null)
            {
                var geoSorting = sortingField as GeoDistanceSortingField;

                var fieldName = geoSorting == null
                    ? availableField.Name
                    : AzureSearchHelper.GetGeoDistanceExpression(availableField.Name, geoSorting.Location);

                result = string.Join(" ", fieldName, sortingField.IsDescending ? "desc" : "asc");
            }

            return result;
        }

        private static string GetFilters(SearchRequest request, IList<Field> availableFields)
        {
            return GetFilterExpressionRecursive(request.Filter, availableFields);
        }

        private static string GetFilterExpressionRecursive(IFilter filter, IList<Field> availableFields)
        {
            string result = null;

            switch (filter)
            {
                case IdsFilter idsFilter:
                    result = CreateIdsFilter(idsFilter, availableFields);
                    break;
                case TermFilter termFilter:
                    result = CreateTermFilter(termFilter, availableFields);
                    break;
                case RangeFilter rangeFilter:
                    result = CreateRangeFilter(rangeFilter, availableFields);
                    break;
                case GeoDistanceFilter geoDistanceFilter:
                    result = CreateGeoDistanceFilter(geoDistanceFilter, availableFields);
                    break;
                case NotFilter notFilter:
                    result = CreateNotFilter(notFilter, availableFields);
                    break;
                case AndFilter andFilter:
                    result = CreateAndFilter(andFilter, availableFields);
                    break;
                case OrFilter orFilter:
                    result = CreateOrFilter(orFilter, availableFields);
                    break;
            }

            return result;
        }

        private static string CreateIdsFilter(IdsFilter idsFilter, IList<Field> availableFields)
        {
            string result = null;

            if (idsFilter?.Values != null)
            {
                var availableField = availableFields.Get(AzureSearchHelper.RawKeyFieldName);
                result = GetEqualsFilterExpression(availableField, idsFilter.Values);
            }

            return result;
        }

        private static string CreateTermFilter(TermFilter termFilter, IList<Field> availableFields)
        {
            string result;

            var availableField = availableFields.Get(termFilter.FieldName);
            if (availableField != null)
            {
                result = availableField.Type.ToString().StartsWith("Collection(")
                    ? GetContainsFilterExpression(availableField, termFilter.Values)
                    : GetEqualsFilterExpression(availableField, termFilter.Values);
            }
            else
            {
                result = AzureSearchHelper.NonExistentFieldFilter;
            }

            return result;
        }

        private static string CreateRangeFilter(RangeFilter rangeFilter, IList<Field> availableFields)
        {
            string result;

            var availableField = availableFields.Get(rangeFilter.FieldName);
            if (availableField != null)
            {
                var expressions = rangeFilter.Values
                    .Select(v => GetRangeFilterValueExpression(v, availableField.Name))
                    .Where(e => !string.IsNullOrEmpty(e))
                    .ToArray();

                result = AzureSearchHelper.JoinNonEmptyStrings(" or ", true, expressions);
            }
            else
            {
                result = AzureSearchHelper.NonExistentFieldFilter;
            }

            return result;
        }

        private static string CreateGeoDistanceFilter(GeoDistanceFilter geoDistanceFilter, IList<Field> availableFields)
        {
            string result;

            var availableField = availableFields.Get(geoDistanceFilter.FieldName);
            if (availableField != null)
            {
                var distance = AzureSearchHelper.GetGeoDistanceExpression(availableField.Name, geoDistanceFilter.Location);
                result = string.Format(CultureInfo.InvariantCulture, "{0} le {1}", distance, geoDistanceFilter.Distance);
            }
            else
            {
                result = AzureSearchHelper.NonExistentFieldFilter;
            }

            return result;
        }

        private static string CreateNotFilter(NotFilter notFilter, IList<Field> availableFields)
        {
            string result = null;

            var childExpression = GetFilterExpressionRecursive(notFilter.ChildFilter, availableFields);
            if (childExpression != null)
            {
                result = $"not ({childExpression})";
            }

            return result;
        }

        private static string CreateAndFilter(AndFilter andFilter, IList<Field> availableFields)
        {
            string result = null;

            if (andFilter.ChildFilters != null)
            {
                var childExpressions = andFilter.ChildFilters.Select(q => GetFilterExpressionRecursive(q, availableFields)).ToArray();
                result = AzureSearchHelper.JoinNonEmptyStrings(" and ", true, childExpressions);
            }

            return result;
        }

        private static string CreateOrFilter(OrFilter orFilter, IList<Field> availableFields)
        {
            string result = null;

            if (orFilter.ChildFilters != null)
            {
                var childExpressions = orFilter.ChildFilters.Select(q => GetFilterExpressionRecursive(q, availableFields)).ToArray();
                result = AzureSearchHelper.JoinNonEmptyStrings(" or ", true, childExpressions);
            }

            return result;
        }

        private static string GetRangeFilterValueExpression(RangeFilterValue filterValue, string azureFieldName)
        {
            var lowerCondition = filterValue.IncludeLower ? "ge" : "gt";
            var upperCondition = filterValue.IncludeUpper ? "le" : "lt";
            return GetRangeFilterExpression(azureFieldName, filterValue.Lower, lowerCondition, filterValue.Upper, upperCondition);
        }

        private static string GetRangeFilterExpression(string azureFieldName, string lowerBound, string lowerCondition, string upperBound, string upperCondition)
        {
            string result = null;

            if (lowerBound?.Length > 0 && lowerCondition?.Length > 0 || upperBound?.Length > 0 && upperCondition?.Length > 0)
            {
                var builder = new StringBuilder();

                if (lowerBound?.Length > 0)
                {
                    builder.Append($"{azureFieldName} {lowerCondition} {lowerBound}");

                    if (upperBound?.Length > 0)
                    {
                        builder.Append(" and ");
                    }
                }

                if (upperBound?.Length > 0)
                {
                    builder.Append($"{azureFieldName} {upperCondition} {upperBound}");
                }

                result = builder.ToString();
            }

            return result;
        }

        private static string GetEqualsFilterExpression(Field availableField, IEnumerable<string> rawValues)
        {
            var azureFieldName = availableField.Name;
            var values = rawValues.Where(v => !string.IsNullOrEmpty(v)).Select(v => GetFilterValue(availableField, v)).ToArray();
            return AzureSearchHelper.JoinNonEmptyStrings(" or ", true, values.Select(v => $"{azureFieldName} eq {v}").ToArray());
        }

        public static string GetContainsFilterExpression(Field availableField, IEnumerable<string> rawValues)
        {
            var azureFieldName = availableField.Name;
            var values = rawValues.Where(v => !string.IsNullOrEmpty(v)).Select(GetStringFilterValue).ToArray();
            return AzureSearchHelper.JoinNonEmptyStrings(" or ", true, values.Select(v => $"{azureFieldName}/any(v: v eq {v})").ToArray());
        }

        private static string GetFilterValue(Field availableField, string rawValue)
        {
            string result;

            if (availableField?.Type == DataType.Boolean)
            {
                result = rawValue.ToLowerInvariant();
            }
            else if (availableField?.Type != DataType.String)
            {
                result = rawValue;
            }
            else
            {
                result = GetStringFilterValue(rawValue);
            }

            return result;
        }

        private static string GetStringFilterValue(string rawValue)
        {
            return $"'{rawValue.Replace("'", "''")}'";
        }

        private class FacetRequest
        {
            public string Id { get; set; }
            public string FieldName { get; set; }
            public string Filter { get; set; }
            public string Facet { get; set; }
        }

        private static IList<FacetRequest> GetFacets(SearchRequest request, IList<Field> availableFields)
        {
            var result = new List<FacetRequest>();

            if (request.Aggregations != null)
            {
                foreach (var aggregation in request.Aggregations)
                {
                    string facet = null;

                    switch (aggregation)
                    {
                        case TermAggregationRequest termAggregationRequest:
                            facet = CreateTermAggregationRequest(termAggregationRequest, availableFields);
                            break;
                        case RangeAggregationRequest rangeAggregationRequest:
                            facet = CreateRangeAggregationRequest(rangeAggregationRequest, availableFields);
                            break;
                    }

                    if (aggregation.Filter != null || !string.IsNullOrEmpty(facet))
                    {
                        var facetRequest = new FacetRequest
                        {
                            Id = aggregation.Id,
                            FieldName = aggregation.FieldName,
                            Filter = GetFilterExpressionRecursive(aggregation.Filter, availableFields),
                            Facet = facet,
                        };

                        result.Add(facetRequest);
                    }
                }
            }

            return result;
        }

        private static string CreateTermAggregationRequest(TermAggregationRequest termAggregationRequest, IList<Field> availableFields)
        {
            string result = null;

            var availableField = availableFields.Get(termAggregationRequest.FieldName);
            if (availableField != null)
            {
                var builder = new StringBuilder(availableField.Name);

                if (termAggregationRequest.Size != null)
                {
                    builder.AppendFormat(CultureInfo.InvariantCulture, ",count:{0}", termAggregationRequest.Size);
                }

                result = builder.ToString();
            }

            return result;
        }

        private static string CreateRangeAggregationRequest(RangeAggregationRequest rangeAggregationRequest, IList<Field> availableFields)
        {
            string result = null;

            var availableField = availableFields.Get(rangeAggregationRequest.FieldName);
            if (availableField != null)
            {
                var edgeValues = rangeAggregationRequest.Values
                    .SelectMany(v => new[] { ConvertToDecimal(v.Lower), ConvertToDecimal(v.Upper) })
                    .Where(v => v > 0m)
                    .Distinct()
                    .OrderBy(v => v)
                    .Select(v => v.ToString(CultureInfo.InvariantCulture))
                    .ToArray();

                var values = string.Join("|", edgeValues);

                result = $"{availableField.Name},values:{values}";
            }

            return result;
        }

        private static decimal ConvertToDecimal(string input)
        {
            var result = 0m;
            if (decimal.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                result = value;
            }

            return result;
        }
    }
}
