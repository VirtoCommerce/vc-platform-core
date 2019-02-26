using System.Collections.Generic;
using System.Linq;
using Nest;
using Newtonsoft.Json.Linq;
using VirtoCommerce.SearchModule.Core.Model;
using SearchRequest = VirtoCommerce.SearchModule.Core.Model.SearchRequest;

namespace VirtoCommerce.ElasticSearchModule.Data
{
    public static class ElasticSearchResponseBuilder
    {
        public static SearchResponse ToSearchResponse(this ISearchResponse<SearchDocument> response, SearchRequest request, string documentType)
        {
            var result = new SearchResponse
            {
                TotalCount = response.Total,
                Documents = response.Hits.Select(ToSearchDocument).ToArray(),
                Aggregations = GetAggregations(response.Aggregations, request)
            };

            return result;
        }

        public static SearchDocument ToSearchDocument(IHit<SearchDocument> hit)
        {
            var result = new SearchDocument { Id = hit.Id };

            // Copy fields and convert JArray to object[]
            var fields = (IDictionary<string, object>)hit.Source ?? hit.Fields;
            if (fields != null)
            {
                foreach (var kvp in fields)
                {
                    var name = kvp.Key;
                    var value = kvp.Value;

                    if (value is JArray jArray)
                    {
                        value = jArray.ToObject<object[]>();
                    }

                    result.Add(name, value);
                }
            }

            return result;
        }

        private static IList<AggregationResponse> GetAggregations(IReadOnlyDictionary<string, IAggregate> searchResponseAggregations, SearchRequest request)
        {
            var result = new List<AggregationResponse>();

            if (request?.Aggregations != null && searchResponseAggregations != null)
            {
                foreach (var aggregationRequest in request.Aggregations)
                {
                    var aggregation = new AggregationResponse
                    {
                        Id = aggregationRequest.Id ?? aggregationRequest.FieldName,
                        Values = new List<AggregationResponseValue>(),
                    };

                    var termAggregationRequest = aggregationRequest as TermAggregationRequest;
                    var rangeAggregationRequest = aggregationRequest as RangeAggregationRequest;

                    if (termAggregationRequest != null)
                    {
                        AddAggregationValues(aggregation, aggregation.Id, aggregation.Id, searchResponseAggregations);
                    }
                    else if (rangeAggregationRequest?.Values != null)
                    {
                        foreach (var value in rangeAggregationRequest.Values)
                        {
                            var queryValueId = value.Id;
                            var responseValueId = $"{aggregation.Id}-{queryValueId}";
                            AddAggregationValues(aggregation, responseValueId, queryValueId, searchResponseAggregations);
                        }
                    }

                    if (aggregation.Values.Any())
                    {
                        result.Add(aggregation);
                    }
                }
            }

            return result;
        }

        private static void AddAggregationValues(AggregationResponse aggregation, string responseKey, string valueId, IReadOnlyDictionary<string, IAggregate> searchResponseAggregations)
        {
            if (searchResponseAggregations.ContainsKey(responseKey))
            {
                var aggregate = searchResponseAggregations[responseKey];
                var bucketAggregate = aggregate as BucketAggregate;
                var singleBucketAggregate = aggregate as SingleBucketAggregate;

                if (singleBucketAggregate != null)
                {
                    if (singleBucketAggregate.Aggregations != null)
                    {
                        bucketAggregate = singleBucketAggregate.Aggregations[responseKey] as BucketAggregate;
                    }
                    else if (singleBucketAggregate.DocCount > 0)
                    {
                        var aggregationValue = new AggregationResponseValue
                        {
                            Id = valueId,
                            Count = singleBucketAggregate.DocCount
                        };

                        aggregation.Values.Add(aggregationValue);
                    }
                }

                if (bucketAggregate != null)
                {
                    foreach (var term in bucketAggregate.Items.OfType<KeyedBucket<object>>())
                    {
                        if (term.DocCount > 0)
                        {
                            var aggregationValue = new AggregationResponseValue
                            {
                                Id = term.Key.ToStringInvariant(),
                                Count = term.DocCount ?? 0
                            };

                            aggregation.Values.Add(aggregationValue);
                        }
                    }
                }
            }
        }
    }
}
