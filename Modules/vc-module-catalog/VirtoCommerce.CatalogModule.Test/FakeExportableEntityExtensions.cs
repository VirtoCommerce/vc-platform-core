using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Test
{
    public static class FakeExportableEntityExtensions
    {
        public static IEnumerable<T> CreateFakeExportables<T>(this string[] values) where T : Entity, IExportable, new()
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

        public static GenericSearchResult<T> EmulateSearch<T>(this string[] values, SearchCriteriaBase searchCriteria) where T : Entity, IExportable, new()
        {
            return values.CreateFakeExportables<T>().EmulateSearch(searchCriteria);
        }

        public static GenericSearchResult<T> EmulateSearch<T>(this IEnumerable<T> exportables, SearchCriteriaBase searchCriteria) where T : Entity, IExportable
        {
            var result = new GenericSearchResult<T>
            {
                TotalCount = exportables.Count(),
                Results = exportables.Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToList()
            };
            return result;
        }
    }
}
