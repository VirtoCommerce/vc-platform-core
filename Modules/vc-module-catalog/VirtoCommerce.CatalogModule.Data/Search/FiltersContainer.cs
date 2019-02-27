using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public class FiltersContainer
    {
        public virtual IList<IFilter> PermanentFilters { get; set; } = new List<IFilter>();
        public virtual IList<KeyValuePair<string, IFilter>> RemovableFilters { get; set; } = new List<KeyValuePair<string, IFilter>>();

        public virtual IList<IFilter> GetFiltersExceptSpecified(string excludeFieldName)
        {
            var removableFilters = RemovableFilters
                .Where(kvp => !kvp.Key.EqualsInvariant(excludeFieldName))
                .Select(kvp => kvp.Value)
                .ToList();

            var result = PermanentFilters.Concat(removableFilters).Where(f => f != null).ToList();
            return result;
        }
    }
}
