using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.LuceneSearchModule.Data
{
    public class LuceneSearchRequestBuilder
    {
        private const Lucene.Net.Util.LuceneVersion _matchVersion = Lucene.Net.Util.LuceneVersion.LUCENE_48;
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
                var searchKeywords = string.Empty;

                //https://stackoverflow.com/questions/48891716/lucene-net-4-8-search-not-returning-results
                var sb = new StringBuilder(request.SearchKeywords);
                if (!request.SearchKeywords.StartsWith(WildcardQuery.WILDCARD_STRING))
                {
                    sb.Insert(0, WildcardQuery.WILDCARD_STRING);
                }
                if (!request.SearchKeywords.EndsWith(WildcardQuery.WILDCARD_STRING))
                {
                    sb.Append(WildcardQuery.WILDCARD_STRING);
                }
                searchKeywords = sb.ToString();
                

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
                    DefaultOperator = QueryParserBase.AND_OPERATOR,
                    AllowLeadingWildcard = true
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
