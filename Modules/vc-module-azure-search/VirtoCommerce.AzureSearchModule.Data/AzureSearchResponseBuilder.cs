using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Search.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.AzureSearchModule.Data
{
    public static class AzureSearchResponseBuilder
    {
        public static SearchResponse ToSearchResponse(this IList<AzureSearchResult> searchResults, SearchRequest request, string documentType)
        {
            var primaryResponse = searchResults.First().ProviderResponse;

            var result = new SearchResponse
            {
                TotalCount = primaryResponse.Count ?? 0,
                Documents = primaryResponse.Results.Select(ToSearchDocument).ToArray(),
                Aggregations = GetAggregations(searchResults, request)
            };

            return result;
        }

        public static SearchDocument ToSearchDocument(SearchResult searchResult)
        {
            var result = new SearchDocument();

            foreach (var (docKey, docValue) in searchResult.Document)
            {
                var key = AzureSearchHelper.FromAzureFieldName(docKey);

                if (key.EqualsInvariant(AzureSearchHelper.RawKeyFieldName))
                {
                    result.Id = docValue.ToStringInvariant();
                }
                else
                {
                    var value = docValue;

                    // Convert DateTimeOffset to DateTime
                    if (value is DateTimeOffset dateTimeOffset)
                    {
                        value = dateTimeOffset.UtcDateTime;
                    }

                    result[key] = value;
                }
            }

            return result;
        }

        private static IList<AggregationResponse> GetAggregations(IList<AzureSearchResult> searchResults, SearchRequest request)
        {
            var result = new List<AggregationResponse>();

            // Combine facets from all responses to a single FacetResults
            var facetResults = searchResults.Where(r => r.ProviderResponse.Facets != null).SelectMany(r => r.ProviderResponse.Facets).ToList();
            if (facetResults.Any())
            {
                var facets = new FacetResults();
                foreach (var keyValuePair in facetResults)
                {
                    facets[keyValuePair.Key] = keyValuePair.Value;
                }

                var responses = request.Aggregations
                    .Select(a => GetAggregation(a, facets))
                    .Where(a => a != null && a.Values.Any());

                result.AddRange(responses);
            }

            // Add responses for aggregations with empty field name
            foreach (var searchResult in searchResults.Where(r => !string.IsNullOrEmpty(r.AggregationId) && r.ProviderResponse.Count > 0))
            {
                result.Add(new AggregationResponse
                {
                    Id = searchResult.AggregationId,
                    Values = new List<AggregationResponseValue>
                        {
                            new AggregationResponseValue
                            {
                                Id= searchResult.AggregationId,
                                Count = searchResult.ProviderResponse.Count ?? 0,
                            }
                        }
                });
            }

            return result;
        }

        private static AggregationResponse GetAggregation(AggregationRequest aggregationRequest, FacetResults facets)
        {
            AggregationResponse result = null;

            switch (aggregationRequest)
            {
                case TermAggregationRequest termAggregationRequest:
                    result = GetTermAggregation(termAggregationRequest, facets);
                    break;
                case RangeAggregationRequest rangeAggregationRequest:
                    result = GetRangeAggregation(rangeAggregationRequest, facets);
                    break;
            }

            return result;
        }

        private static AggregationResponse GetTermAggregation(TermAggregationRequest termAggregationRequest, FacetResults facets)
        {
            AggregationResponse result = null;

            if (termAggregationRequest != null)
            {
                var azureFieldName = AzureSearchHelper.ToAzureFieldName(termAggregationRequest.FieldName);
                if (!string.IsNullOrEmpty(azureFieldName))
                {
                    var facetResults = facets.ContainsKey(azureFieldName) ? facets[azureFieldName] : null;

                    if (facetResults != null && facetResults.Any())
                    {
                        result = new AggregationResponse
                        {
                            Id = (termAggregationRequest.Id ?? termAggregationRequest.FieldName).ToLowerInvariant(),
                            Values = new List<AggregationResponseValue>(),
                        };

                        var values = termAggregationRequest.Values;

                        if (values != null)
                        {
                            foreach (var value in values)
                            {
                                var facetResult = facetResults.FirstOrDefault(r => r.Value.ToStringInvariant().EqualsInvariant(value));
                                AddAggregationValue(result, facetResult, value);
                            }
                        }
                        else
                        {
                            // Return all facet results if values are not defined
                            foreach (var facetResult in facetResults)
                            {
                                var aggregationValue = new AggregationResponseValue
                                {
                                    Id = facetResult.Value.ToStringInvariant(),
                                    Count = facetResult.Count ?? 0,
                                };
                                result.Values.Add(aggregationValue);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static AggregationResponse GetRangeAggregation(RangeAggregationRequest rangeAggregationRequest, FacetResults facets)
        {
            AggregationResponse result = null;

            if (rangeAggregationRequest != null)
            {
                var azureFieldName = AzureSearchHelper.ToAzureFieldName(rangeAggregationRequest.FieldName);
                if (!string.IsNullOrEmpty(azureFieldName))
                {
                    var facetResults = facets.ContainsKey(azureFieldName) ? facets[azureFieldName] : null;

                    if (facetResults != null && facetResults.Any())
                    {
                        result = new AggregationResponse
                        {
                            Id = (rangeAggregationRequest.Id ?? rangeAggregationRequest.FieldName).ToLowerInvariant(),
                            Values = new List<AggregationResponseValue>(),
                        };

                        foreach (var value in rangeAggregationRequest.Values)
                        {
                            var facetResult = GetRangeFacetResult(value, facetResults);
                            AddAggregationValue(result, facetResult, value.Id);
                        }
                    }
                }
            }

            return result;
        }

        private static FacetResult GetRangeFacetResult(RangeAggregationRequestValue value, IEnumerable<FacetResult> facetResults)
        {
            var lower = value.Lower == null ? null : value.Lower.Length == 0 ? null : value.Lower == "0" ? null : value.Lower;
            var upper = value.Upper;

            return facetResults.FirstOrDefault(r => r.Count > 0 && r.From?.ToStringInvariant() == lower && r.To?.ToStringInvariant() == upper);
        }

        private static void AddAggregationValue(AggregationResponse aggregation, FacetResult facetResult, string valueId)
        {
            if (facetResult != null && facetResult.Count > 0)
            {
                var aggregationValue = new AggregationResponseValue
                {
                    Id = valueId,
                    Count = facetResult.Count ?? 0,
                };
                aggregation.Values.Add(aggregationValue);
            }
        }
    }
}
