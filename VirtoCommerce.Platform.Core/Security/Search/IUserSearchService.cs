using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security.Search
{
    public interface IUserSearchService
    {
        Task<GenericSearchResult<ApplicationUser>> SearchUsersAsync(UserSearchCriteria criteria);

    }
}
