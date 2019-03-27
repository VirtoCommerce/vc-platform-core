using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.Search;

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

            var sortInfos = GetSearchUsersSortInfo(criteria);
            var query = GetSearchUsersQuery(criteria, sortInfos);

            var result = new GenericSearchResult<ApplicationUser>
            {
                TotalCount = await query.CountAsync(),
                Results = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync()
            };

            return result;
        }

        private IList<SortInfo> GetSearchUsersSortInfo(UserSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = ReflectionUtility.GetPropertyName<ApplicationUser>(x => x.UserName),
                        SortDirection = SortDirection.Descending
                    }
                };
            }

            return sortInfos;
        }

        private IQueryable<ApplicationUser> GetSearchUsersQuery(UserSearchCriteria criteria, IList<SortInfo> sortInfos)
        {
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

            query = query.OrderBySortInfos(sortInfos);

            return query;
        }
    }
}
