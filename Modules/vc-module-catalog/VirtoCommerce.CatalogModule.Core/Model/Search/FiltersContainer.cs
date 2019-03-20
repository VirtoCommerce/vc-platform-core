using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
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
