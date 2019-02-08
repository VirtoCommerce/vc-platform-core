using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace VirtoCommerce.Platform.Core.Common
{
    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBySortInfos<T>(this IQueryable<T> source, IEnumerable<SortInfo> sortInfos)
        {
            if (sortInfos.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(sortInfos));
            }

            var orderString = string.Join(",", sortInfos.Select(GetSortString));
            return source.OrderBy(orderString);
        }

        private static string GetSortString(SortInfo sortInfo)
        {
            var direction = sortInfo.SortDirection == SortDirection.Ascending ? "asc" : "desc";
            return $"{sortInfo.SortColumn} {direction}";
        }
    }
}
