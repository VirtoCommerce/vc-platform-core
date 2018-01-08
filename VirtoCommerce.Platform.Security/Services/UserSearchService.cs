using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.Search;
using Microsoft.EntityFrameworkCore;

namespace VirtoCommerce.Platform.Security.Services
{
    public class UserSearchService : IUserSearchService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserSearchService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<GenericSearchResult<ApplicationUser>> SearchUsersAsync(UserSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }
            if (!_userManager.SupportsQueryableUsers)
            {
                throw new NotSupportedException();
            }

            var result = new GenericSearchResult<ApplicationUser>();
            var query = _userManager.Users;
            if (criteria.Keyword != null)
            {
                query = query.Where(r => r.UserName.Contains(criteria.Keyword));
            }

            if(!string.IsNullOrEmpty(criteria.MemberId))
            {
                query = query.Where(r => r.MemberId == criteria.MemberId);
            }
            result.TotalCount = await query.CountAsync();

            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<ApplicationUser>(x => x.UserName), SortDirection = SortDirection.Descending } };
            }
            result.Results = await query.OrderBySortInfos(sortInfos).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();

            return result;
        }
    }
}
