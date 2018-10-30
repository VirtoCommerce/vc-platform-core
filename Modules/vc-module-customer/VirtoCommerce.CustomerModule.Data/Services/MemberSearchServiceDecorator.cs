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
        public MemberSearchServiceDecorator(CommerceMembersSearchServiceImpl memberSearchService, MemberIndexedSearchService memberIndexedSearchService)
        {
            MemberSearchService = memberSearchService;
            MemberIndexedSearchService = memberIndexedSearchService;
        }

        protected CommerceMembersSearchServiceImpl MemberSearchService { get; }
        protected MemberIndexedSearchService MemberIndexedSearchService { get; }

        public virtual Task<GenericSearchResult<Member>> SearchMembersAsync(MembersSearchCriteria criteria)
        {
            var result = !string.IsNullOrEmpty(criteria?.Keyword)
                ? SearchIndexAsync(criteria)
                : MemberSearchService.SearchMembersAsync(criteria);

            return result;
        }

        protected virtual Task<GenericSearchResult<Member>> SearchIndexAsync(MembersSearchCriteria criteria)
        {
            return MemberIndexedSearchService.SearchAsync(criteria);
        }
    }
}
