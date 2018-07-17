using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Core.Services
{
    public interface IMemberSearchService
    {
        Task<GenericSearchResult<Member>> SearchMembersAsync(MembersSearchCriteria criteria);
    }
}
