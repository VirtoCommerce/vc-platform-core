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
        public async Task<UserSearchResult> SearchUsersAsync(UserSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }
            if (!_userManager.SupportsQueryableUsers)
            {
                throw new NotSupportedException();
            }

            var result = AbstractTypeFactory<UserSearchResult>.TryCreateInstance();
            var query = _userManager.Users;
            if (criteria.Keyword != null)
            {
                query = query.Where(x => x.UserName.Contains(criteria.Keyword));
            }

            if (!string.IsNullOrEmpty(criteria.MemberId))
            {
                query = query.Where(x => x.MemberId == criteria.MemberId);
            }

            if (!criteria.MemberIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.MemberIds.Contains(x.MemberId));
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
