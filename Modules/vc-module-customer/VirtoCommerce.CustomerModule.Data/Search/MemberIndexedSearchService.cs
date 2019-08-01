using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerModule.Core.Services.Indexed;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.CustomerModule.Data.Search
{
    public class MemberIndexedSearchService : IIndexedMemberSearchService
    {
        private readonly IEnumerable<ISearchRequestBuilder> _searchRequestBuilders;
        private readonly ISearchProvider _searchProvider;
        private readonly IMemberService _memberService;

        public MemberIndexedSearchService(
            IEnumerable<ISearchRequestBuilder> searchRequestBuilders
            , ISearchProvider searchProvider
            , IMemberService memberService)
        {
            _searchRequestBuilders = searchRequestBuilders;
            _searchProvider = searchProvider;
            _memberService = memberService;
        }

        public virtual async Task<MemberSearchResult> SearchMembersAsync(MembersSearchCriteria criteria)
        {
            var requestBuilder = GetRequestBuilder(criteria);
            var request = await requestBuilder?.BuildRequestAsync(criteria);

            var response = await _searchProvider.SearchAsync(criteria.ObjectType, request);

            var result = await ConvertResponseAsync(response, criteria);
            return result;
        }

        protected virtual ISearchRequestBuilder GetRequestBuilder(MembersSearchCriteria criteria)
        {
            var requestBuilder = _searchRequestBuilders?.FirstOrDefault(b => b.DocumentType.Equals(criteria.ObjectType)) ??
                                 _searchRequestBuilders?.FirstOrDefault(b => string.IsNullOrEmpty(b.DocumentType));

            if (requestBuilder == null)
                throw new InvalidOperationException($"No query builders found for document type '{criteria.ObjectType}'");

            return requestBuilder;
        }

        protected virtual async Task<MemberSearchResult> ConvertResponseAsync(SearchResponse response, MembersSearchCriteria criteria)
        {
            var result = AbstractTypeFactory<MemberSearchResult>.TryCreateInstance();

            if (response != null)
            {
                result.TotalCount = (int)response.TotalCount;
                result.Results = await ConvertDocumentsAsync(response.Documents, criteria);
            }

            return result;
        }

        protected virtual async Task<IList<Member>> ConvertDocumentsAsync(IList<SearchDocument> documents, MembersSearchCriteria criteria)
        {
            var result = new List<Member>();

            if (documents?.Any() == true)
            {
                var itemIds = documents.Select(doc => doc.Id).ToArray();
                var items = await GetMembersByIdsAsync(itemIds, criteria);
                var itemsMap = items.ToDictionary(m => m.Id, m => m);

                // Preserve documents order
                var members = documents
                    .Select(doc => itemsMap.ContainsKey(doc.Id) ? itemsMap[doc.Id] : null)
                    .Where(m => m != null)
                    .ToArray();

                result.AddRange(members);
            }

            return result;
        }

        protected virtual async Task<IList<Member>> GetMembersByIdsAsync(IList<string> itemIds, MembersSearchCriteria criteria)
        {
            var result = await _memberService.GetByIdsAsync(itemIds.ToArray(), criteria.ResponseGroup, criteria.MemberTypes);
            return result;
        }
    }
}
