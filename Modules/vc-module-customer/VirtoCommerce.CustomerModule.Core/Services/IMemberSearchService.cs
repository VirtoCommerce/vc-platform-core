using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model.Search;

namespace VirtoCommerce.CustomerModule.Core.Services
{
    public interface IMemberSearchService
    {
        Task<MemberSearchResult> SearchMembersAsync(MembersSearchCriteria criteria);
    }
}
