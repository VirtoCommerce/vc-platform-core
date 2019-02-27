using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Catalog.Model.Search;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public static class SearchCriteriaExtensions
    {
        public static IList<string> GetPriorityFields(this CatalogSearchCriteriaBase criteria)
        {
            var allNames = criteria
                .GetRawOutlines()
                .Select(outline => StringsHelper.JoinNonEmptyStrings("_", "priority", criteria.CatalogId, outline.Split('/').LastOrDefault()).ToLowerInvariant())
                .ToList();

            allNames.Add("priority");

            var result = allNames.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            return result;
        }

        public static IList<string> GetOutlines(this CatalogSearchCriteriaBase criteria)
        {
            var result = criteria
                .GetRawOutlines()
                .Select(outline => StringsHelper.JoinNonEmptyStrings("/", criteria.CatalogId, outline).ToLowerInvariant())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return result;
        }

        public static IList<string> GetRawOutlines(this CatalogSearchCriteriaBase criteria)
        {
            var outlines = new List<string>();

            if (!string.IsNullOrEmpty(criteria.Outline))
            {
                outlines.Add(criteria.Outline);
            }

            if (criteria.Outlines?.Any() == true)
            {
                outlines.AddRange(criteria.Outlines);
            }

            var result = outlines
                .Where(o => !string.IsNullOrEmpty(o))
                .Select(o => o.TrimEnd('*'))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return result;
        }

        public static IList<StringKeyValues> GetTerms(this CatalogSearchCriteriaBase criteria)
        {
            var result = new List<StringKeyValues>();

            if (criteria.Terms != null)
            {
                var nameValueDelimeter = new[] { ':' };
                var valuesDelimeter = new[] { ',' };

                result.AddRange(criteria.Terms
                    .Select(item => item.Split(nameValueDelimeter, 2))
                    .Where(item => item.Length == 2)
                    .Select(item => new StringKeyValues { Key = item[0], Values = item[1].Split(valuesDelimeter, StringSplitOptions.RemoveEmptyEntries) }));
            }

            return result;
        }
    }

    public class StringKeyValues
    {
        public string Key { get; set; }
        public string[] Values { get; set; }
    }
}
