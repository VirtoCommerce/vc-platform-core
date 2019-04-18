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
    public class RoleSearchService : IRoleSearchService
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleSearchService(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<RoleSearchResult> SearchRolesAsync(RoleSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }
            if (!_roleManager.SupportsQueryableRoles)
            {
                throw new NotSupportedException();
            }

            var result = AbstractTypeFactory<RoleSearchResult>.TryCreateInstance();
            var sortInfos = GetSearchSortInfos(criteria);
            var query = GetSearchQuery(criteria, sortInfos);

            result.TotalCount = await query.CountAsync();
            result.Results = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();

            return result;
        }

        protected virtual IList<SortInfo> GetSearchSortInfos(RoleSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<Role>(x => x.Name), SortDirection = SortDirection.Descending } };
            }

            return sortInfos;
        }

        protected virtual IQueryable<Role> GetSearchQuery(RoleSearchCriteria criteria, IList<SortInfo> sortInfos)
        {
            var query = _roleManager.Roles;

            if (criteria.Keyword != null)
            {
                query = query.Where(r => r.Name.Contains(criteria.Keyword));
            }

            query = query.OrderBySortInfos(sortInfos);
            return query;
        }
    }
}
