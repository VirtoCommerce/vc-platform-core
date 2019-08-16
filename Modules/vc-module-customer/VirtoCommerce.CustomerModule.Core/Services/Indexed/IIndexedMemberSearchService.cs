using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model.Search;

namespace VirtoCommerce.CustomerModule.Core.Services.Indexed
{
    public interface IIndexedMemberSearchService
    {
        Task<MemberSearchResult> SearchMembersAsync(MembersSearchCriteria criteria);
    }
}
