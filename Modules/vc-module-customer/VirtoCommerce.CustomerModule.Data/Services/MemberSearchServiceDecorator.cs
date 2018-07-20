using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerModule.Data.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Services
{
    public class MemberSearchServiceDecorator : IMemberSearchService
    {
        private readonly CommerceMembersSearchServiceImpl _memberSearchService;
        private readonly MemberIndexedSearchService _memberIndexedSearchService;

        public MemberSearchServiceDecorator(CommerceMembersSearchServiceImpl memberSearchService, MemberIndexedSearchService memberIndexedSearchService)
        {
            _memberSearchService = memberSearchService;
            _memberIndexedSearchService = memberIndexedSearchService;
        }

        public virtual Task<GenericSearchResult<Member>> SearchMembersAsync(MembersSearchCriteria criteria)
        {
            var result = !string.IsNullOrEmpty(criteria?.Keyword)
                ? SearchIndexAsync(criteria)
                : _memberSearchService.SearchMembersAsync(criteria);

            return result;
        }

        protected virtual Task<GenericSearchResult<Member>> SearchIndexAsync(MembersSearchCriteria criteria)
        {
            return _memberIndexedSearchService.SearchAsync(criteria);
        }
    }
}
