using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using VirtoCommerce.SearchModule.Core.Model;
using Version = Lucene.Net.Util.LuceneVersion;

namespace VirtoCommerce.LuceneSearchModule.Data
{
    public class LuceneSearchRequestBuilder
    {
        private const Version _matchVersion = Version.LUCENE_48;
        private static readonly string[] _keywordSeparator = { " " };

        public static LuceneSearchRequest BuildRequest(SearchRequest request, string indexName, string documentType, ICollection<string> availableFields)
        {
            var query = GetQuery(request);
            var filters = GetFilters(request, availableFields);

            var result = new LuceneSearchRequest
            {
                Query = query?.ToString().Equals(string.Empty) == true ? new MatchAllDocsQuery() : query,
                Filter = filters?.ToString().Equals("BooleanFilter()") == true ? null : filters,
                Sort = GetSorting(request, availableFields),
                Count = request.Take + request.Skip,
            };

            return result;
        }

        private static Sort GetSorting(SearchRequest request, ICollection<string> availableFields)
        {
            Sort result = null;

            if (request?.Sorting?.Any() == true)
            {
                result = new Sort(request.Sorting.Select(f => GetSortField(f, availableFields)).ToArray());
            }

            return result;
        }

        private static SortField GetSortField(SortingField field, ICollection<string> availableFields)
        {
            var dataType = availableFields.Contains(LuceneSearchHelper.GetFacetableFieldName(field.FieldName)) ? SortFieldType.DOUBLE : SortFieldType.STRING;
            var result = new SortField(LuceneSearchHelper.ToLuceneFieldName(field.FieldName), dataType, field.IsDescending);
            return result;
        }

        private static Query GetQuery(SearchRequest request)
        {
            Query result = null;

            if (!string.IsNullOrEmpty(request?.SearchKeywords))
            {
                var searchKeywords = request.SearchKeywords;

                //https://stackoverflow.com/questions/48891716/lucene-net-4-8-search-not-returning-results
                //and also not allowed as first character in WildcardQuery
                if (!searchKeywords.EndsWith(WildcardQuery.WILDCARD_STRING))
                {
                    searchKeywords = $"{searchKeywords}{WildcardQuery.WILDCARD_STRING}";
                }

                if (request.IsFuzzySearch)
                {
                    const string fuzzyMinSimilarity = "0.7";
                    var keywords = searchKeywords.Replace("~", string.Empty).Split(_keywordSeparator, StringSplitOptions.RemoveEmptyEntries);

                    searchKeywords = string.Empty;
                    searchKeywords = keywords.Aggregate(searchKeywords, (current, keyword) => current + $"{keyword}~{fuzzyMinSimilarity}");
                }
                
                var fields = request.SearchFields?.Select(LuceneSearchHelper.ToLuceneFieldName).ToArray() ?? LuceneSearchHelper.SearchableFields;
                var analyzer = new StandardAnalyzer(_matchVersion);

                var parser = new MultiFieldQueryParser(_matchVersion, fields, analyzer)
                {
                    DefaultOperator = QueryParserBase.AND_OPERATOR
                };

                result = parser.Parse(searchKeywords);
            }

            return result;
        }

        private static Filter GetFilters(SearchRequest request, ICollection<string> availableFields)
        {
            return LuceneSearchFilterBuilder.GetFilterRecursive(request.Filter, availableFields);
        }
    }
}
