using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Tests.ComplexExportPagedDataSourceTests
{
    public static class TestExportableEntityExtensions
    {
        public static GenericSearchResult<IExportable> EmulateSearch<T>(this string[] values, SearchCriteriaBase searchCriteria) where T : IExportable, new()
        {
            return values.CreateTestExportables<T>().EmulateSearch(searchCriteria);
        }

        private static GenericSearchResult<IExportable> EmulateSearch<T>(this IEnumerable<T> exportables, SearchCriteriaBase searchCriteria) where T : IExportable
        {
            var result = new GenericSearchResult<IExportable>
            {
                TotalCount = exportables.Count(),
                Results = exportables.Cast<IExportable>().Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToList()
            };
            return result;
        }

        private static IEnumerable<T> CreateTestExportables<T>(this string[] values) where T : IExportable, new()
        {
            return values.Select(x =>
            {
                var t = new T
                {
                    Id = x
                };
                return t;
            }
            );
        }
    }
}
