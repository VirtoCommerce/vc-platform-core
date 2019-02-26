using System.Collections.Generic;
using System.Linq;
using Nest;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using SearchRequest = VirtoCommerce.SearchModule.Core.Model.SearchRequest;

namespace VirtoCommerce.ElasticSearchModule.Data
{
    public class ElasticSearchRequestBuilder
    {
        public virtual ISearchRequest BuildRequest(SearchRequest request, string indexName, string documentType, Properties<IProperties> availableFields)
        {
            var result = new Nest.SearchRequest(indexName, documentType)
            {
                Query = GetQuery(request),
                PostFilter = GetFilters(request, availableFields),
                Aggregations = GetAggregations(request, availableFields),
                Sort = GetSorting(request?.Sorting),
                From = request?.Skip,
                Size = request?.Take,
            };

            return result;
        }

        protected virtual QueryContainer GetQuery(SearchRequest request)
        {
            QueryContainer result = null;

            if (!string.IsNullOrEmpty(request?.SearchKeywords))
            {
                var keywords = request.SearchKeywords;
                var fields = request.SearchFields?.Select(ElasticSearchHelper.ToElasticFieldName).ToArray() ?? new[] { "_all" };

                var multiMatch = new MultiMatchQuery
                {
                    Fields = fields,
                    Query = keywords,
                    Analyzer = "standard",
                    Operator = Operator.And,
                };

                if (request.IsFuzzySearch)
                {
                    multiMatch.Fuzziness = request.Fuzziness != null ? Fuzziness.EditDistance(request.Fuzziness.Value) : Fuzziness.Auto;
                }

                result = multiMatch;
            }

            return result;
        }

        protected virtual IList<ISort> GetSorting(IEnumerable<SortingField> fields)
        {
            var result = fields?.Select(GetSortingField).ToArray();
            return result;
        }

        protected virtual ISort GetSortingField(SortingField field)
        {
            ISort result;

            var geoSorting = field as GeoDistanceSortingField;

            if (geoSorting != null)
            {
                result = new GeoDistanceSort
                {
                    Field = ElasticSearchHelper.ToElasticFieldName(field.FieldName),
                    Points = new[] { geoSorting.Location.ToGeoLocation() },
                    Order = geoSorting.IsDescending ? SortOrder.Descending : SortOrder.Ascending,
                };
            }
            else
            {
                result = new SortField
                {
                    Field = ElasticSearchHelper.ToElasticFieldName(field.FieldName),
                    Order = field.IsDescending ? SortOrder.Descending : SortOrder.Ascending,
                    Missing = "_last",
                    UnmappedType = FieldType.Long,
                };
            }

            return result;
        }

        protected virtual QueryContainer GetFilters(SearchRequest request, Properties<IProperties> availableFields)
        {
            return GetFilterQueryRecursive(request?.Filter, availableFields);
        }

        protected virtual QueryContainer GetFilterQueryRecursive(IFilter filter, Properties<IProperties> availableFields)
        {
            QueryContainer result = null;

            var idsFilter = filter as IdsFilter;
            var termFilter = filter as TermFilter;
            var rangeFilter = filter as RangeFilter;
            var geoDistanceFilter = filter as GeoDistanceFilter;
            var notFilter = filter as NotFilter;
            var andFilter = filter as AndFilter;
            var orFilter = filter as OrFilter;

            if (idsFilter != null)
            {
                result = CreateIdsFilter(idsFilter);
            }
            else if (termFilter != null)
            {
                result = CreateTermFilter(termFilter, availableFields);
            }
            else if (rangeFilter != null)
            {
                result = CreateRangeFilter(rangeFilter);
            }
            else if (geoDistanceFilter != null)
            {
                result = CreateGeoDistanceFilter(geoDistanceFilter);
            }
            else if (notFilter != null)
            {
                result = CreateNotFilter(notFilter, availableFields);
            }
            else if (andFilter != null)
            {
                result = CreateAndFilter(andFilter, availableFields);
            }
            else if (orFilter != null)
            {
                result = CreateOrFilter(orFilter, availableFields);
            }

            return result;
        }

        protected virtual QueryContainer CreateIdsFilter(IdsFilter idsFilter)
        {
            QueryContainer result = null;

            if (idsFilter?.Values != null)
            {
                result = new IdsQuery { Values = idsFilter.Values.Select(id => new Id(id)) };
            }

            return result;
        }

        protected virtual QueryContainer CreateTermFilter(TermFilter termFilter, Properties<IProperties> availableFields)
        {
            var termValues = termFilter.Values;

            var field = availableFields.Where(kvp => kvp.Key.Name.EqualsInvariant(termFilter.FieldName)).Select(kvp => kvp.Value).FirstOrDefault();
            if (field?.Type?.Name?.EqualsInvariant("boolean") == true)
            {
                termValues = termValues.Select(v => v.ToLowerInvariant()).ToArray();
            }

            return new TermsQuery
            {
                Field = ElasticSearchHelper.ToElasticFieldName(termFilter.FieldName),
                Terms = termValues
            };
        }

        protected virtual QueryContainer CreateRangeFilter(RangeFilter rangeFilter)
        {
            QueryContainer result = null;

            var fieldName = ElasticSearchHelper.ToElasticFieldName(rangeFilter.FieldName);
            foreach (var value in rangeFilter.Values)
            {
                result |= CreateTermRangeQuery(fieldName, value);
            }

            return result;
        }

        protected virtual QueryContainer CreateGeoDistanceFilter(GeoDistanceFilter geoDistanceFilter)
        {
            return new GeoDistanceQuery
            {
                Field = ElasticSearchHelper.ToElasticFieldName(geoDistanceFilter.FieldName),
                Location = geoDistanceFilter.Location.ToGeoLocation(),
                Distance = new Distance(geoDistanceFilter.Distance, DistanceUnit.Kilometers),
            };
        }

        protected virtual QueryContainer CreateNotFilter(NotFilter notFilter, Properties<IProperties> availableFields)
        {
            QueryContainer result = null;

            if (notFilter?.ChildFilter != null)
            {
                result = !GetFilterQueryRecursive(notFilter.ChildFilter, availableFields);
            }

            return result;
        }

        protected virtual QueryContainer CreateAndFilter(AndFilter andFilter, Properties<IProperties> availableFields)
        {
            QueryContainer result = null;

            if (andFilter?.ChildFilters != null)
            {
                foreach (var childQuery in andFilter.ChildFilters)
                {
                    result &= GetFilterQueryRecursive(childQuery, availableFields);
                }
            }

            return result;
        }

        protected virtual QueryContainer CreateOrFilter(OrFilter orFilter, Properties<IProperties> availableFields)
        {
            QueryContainer result = null;

            if (orFilter?.ChildFilters != null)
            {
                foreach (var childQuery in orFilter.ChildFilters)
                {
                    result |= GetFilterQueryRecursive(childQuery, availableFields);
                }
            }

            return result;
        }

        protected virtual TermRangeQuery CreateTermRangeQuery(string fieldName, RangeFilterValue value)
        {
            var lower = string.IsNullOrEmpty(value.Lower) ? null : value.Lower;
            var upper = string.IsNullOrEmpty(value.Upper) ? null : value.Upper;
            return CreateTermRangeQuery(fieldName, lower, upper, value.IncludeLower, value.IncludeUpper);
        }

        protected virtual TermRangeQuery CreateTermRangeQuery(string fieldName, string lower, string upper, bool includeLower, bool includeUpper)
        {
            var termRangeQuery = new TermRangeQuery { Field = fieldName };

            if (includeLower)
            {
                termRangeQuery.GreaterThanOrEqualTo = lower;
            }
            else
            {
                termRangeQuery.GreaterThan = lower;
            }

            if (includeUpper)
            {
                termRangeQuery.LessThanOrEqualTo = upper;
            }
            else
            {
                termRangeQuery.LessThan = upper;
            }

            return termRangeQuery;
        }

        protected virtual AggregationDictionary GetAggregations(SearchRequest request, Properties<IProperties> availableFields)
        {
            var result = new Dictionary<string, AggregationContainer>();

            if (request?.Aggregations != null)
            {
                foreach (var aggregation in request.Aggregations)
                {
                    var aggregationId = aggregation.Id ?? aggregation.FieldName;
                    var fieldName = ElasticSearchHelper.ToElasticFieldName(aggregation.FieldName);
                    var filter = GetFilterQueryRecursive(aggregation.Filter, availableFields);

                    var termAggregationRequest = aggregation as TermAggregationRequest;
                    var rangeAggregationRequest = aggregation as RangeAggregationRequest;

                    if (termAggregationRequest != null)
                    {
                        AddTermAggregationRequest(result, aggregationId, fieldName, filter, termAggregationRequest);
                    }
                    else if (rangeAggregationRequest != null)
                    {
                        AddRangeAggregationRequest(result, aggregationId, fieldName, rangeAggregationRequest.Values);
                    }
                }
            }

            return result.Any() ? new AggregationDictionary(result) : null;
        }

        protected virtual void AddTermAggregationRequest(IDictionary<string, AggregationContainer> container, string aggregationId, string field, QueryContainer filter, TermAggregationRequest termAggregationRequest)
        {
            var facetSize = termAggregationRequest.Size;

            TermsAggregation termsAggregation = null;

            if (!string.IsNullOrEmpty(field))
            {
                termsAggregation = new TermsAggregation(aggregationId)
                {
                    Field = field,
                    Size = facetSize == null ? null : facetSize > 0 ? facetSize : int.MaxValue,
                };

                if (termAggregationRequest.Values?.Any() == true)
                {
                    termsAggregation.Include = new TermsIncludeExclude
                    {
                        Values = termAggregationRequest.Values
                    };
                }
            }

            if (filter == null)
            {
                if (termsAggregation != null)
                {
                    container.Add(aggregationId, termsAggregation);
                }
            }
            else
            {
                var filterAggregation = new FilterAggregation(aggregationId)
                {
                    Filter = filter,
                };

                if (termsAggregation != null)
                {
                    filterAggregation.Aggregations = termsAggregation;
                }

                container.Add(aggregationId, filterAggregation);
            }
        }

        protected virtual void AddRangeAggregationRequest(Dictionary<string, AggregationContainer> container, string aggregationId, string fieldName, IEnumerable<RangeAggregationRequestValue> values)
        {
            if (values == null)
                return;

            foreach (var value in values)
            {
                var aggregationValueId = $"{aggregationId}-{value.Id}";
                var query = CreateTermRangeQuery(fieldName, value.Lower, value.Upper, value.IncludeLower, value.IncludeUpper);

                var filterAggregation = new FilterAggregation(aggregationValueId)
                {
                    Filter = new BoolQuery
                    {
                        Must = new List<QueryContainer> { query }
                    }
                };

                container.Add(aggregationValueId, filterAggregation);
            }
        }
    }
}
