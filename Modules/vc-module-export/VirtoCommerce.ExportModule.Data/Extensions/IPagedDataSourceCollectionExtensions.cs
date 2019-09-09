using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Data.Extensions
{

    public static class IPagedDataSourceCollectionExtensions
    {
        public static IEnumerable<IExportable> GetItems(this IEnumerable<IPagedDataSource> datasources, int skip, int take)
        {
            var taskList = new List<Task<IEnumerable<IExportable>>>();

            foreach (var datasource in datasources)
            {
                var totalCount = datasource.GetTotalCount();

                var currentSkip = Math.Min(totalCount, skip);
                var currentTake = Math.Min(take, Math.Max(0, totalCount - skip));

                if (currentSkip <= 0 && currentTake <= 0)
                {
                    break;
                }
                else if (currentSkip < totalCount && currentTake > 0)
                {
                    datasource.Skip = currentSkip;
                    datasource.Take = currentTake;
                    taskList.Add(Task.Factory.StartNew(() => datasource.Fetch() ? datasource.Items : Array.Empty<IExportable>()));
                }

                skip -= currentSkip;
                take -= currentTake;
            }

            Task.WhenAll(taskList).GetAwaiter().GetResult();

            return taskList.SelectMany(x => x.Result).ToList();
        }
    }
}
