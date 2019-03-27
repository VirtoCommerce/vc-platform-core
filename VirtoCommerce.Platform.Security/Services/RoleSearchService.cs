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

        public async Task<GenericSearchResult<Role>> SearchRolesAsync(RoleSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }
            if (!_roleManager.SupportsQueryableRoles)
            {
                throw new NotSupportedException();
            }

            var sortInfos = GetSearchRolesSortInfo(criteria);
            var query = GetSearchRolesQuery(criteria, sortInfos);

            var result = new GenericSearchResult<Role>
            {
                TotalCount = await query.CountAsync(),
                Results = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync()
            };

            return result;
        }

        private IList<SortInfo> GetSearchRolesSortInfo(RoleSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = ReflectionUtility.GetPropertyName<Role>(x => x.Name),
                        SortDirection = SortDirection.Descending
                    }
                };
            }

            return sortInfos;
        }

        private IQueryable<Role> GetSearchRolesQuery(RoleSearchCriteria criteria, IList<SortInfo> sortInfos)
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
