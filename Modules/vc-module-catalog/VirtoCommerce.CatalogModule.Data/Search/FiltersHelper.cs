using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VirtoCommerce.Domain.Catalog.Model.Search;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public static class FiltersHelper
    {
        public static IFilter CreateOutlineFilter(CatalogSearchCriteriaBase criteria)
        {
            IFilter result = null;

            var outlines = criteria.GetOutlines();
            if (outlines.Any())
            {
                result = CreateTermFilter("__outline", outlines);
            }

            return result;
        }

        public static IFilter CreateTermFilter(string fieldName, string value)
        {
            return new TermFilter
            {
                FieldName = fieldName,
                Values = new[] { value },
            };
        }

        public static IFilter CreateTermFilter(string fieldName, IEnumerable<string> values)
        {
            return new TermFilter
            {
                FieldName = fieldName,
                Values = values.ToArray(),
            };
        }

        public static IFilter CreateDateRangeFilter(string fieldName, DateTime? lower, DateTime? upper, bool includeLower, bool includeUpper)
        {
            return CreateRangeFilter(fieldName, lower?.ToString("O"), upper?.ToString("O"), includeLower, includeUpper);
        }

        public static IFilter CreateRangeFilter(string fieldName, string lower, string upper, bool includeLower, bool includeUpper)
        {
            return new RangeFilter
            {
                FieldName = fieldName,
                Values = new[] { CreateRangeFilterValue(lower, upper, includeLower, includeUpper) },
            };
        }

        public static RangeFilterValue CreateRangeFilterValue(string lower, string upper, bool includeLower, bool includeUpper)
        {
            return new RangeFilterValue
            {
                Lower = lower,
                Upper = upper,
                IncludeLower = includeLower,
                IncludeUpper = includeUpper,
            };
        }

        public static IFilter CreatePriceRangeFilter(string currency, IList<string> pricelists, decimal? lower, decimal? upper, bool includeLower, bool includeUpper)
        {
            var result = CreatePriceRangeFilter(currency, pricelists, lower?.ToString(CultureInfo.InvariantCulture), upper?.ToString(CultureInfo.InvariantCulture), includeLower, includeUpper);
            return result;
        }

        public static IFilter CreatePriceRangeFilter(string currency, IList<string> pricelists, string lower, string upper, bool includeLower, bool includeUpper)
        {
            var commonFieldName = StringsHelper.JoinNonEmptyStrings("_", "price", currency).ToLowerInvariant();
            var result = GetPriceRangeFilterRecursive(0, commonFieldName, pricelists, lower, upper, includeLower, includeUpper);
            return result;
        }


        private static IFilter GetPriceRangeFilterRecursive(int pricelistNumber, string commonFieldName, IList<string> pricelists, string lower, string upper, bool includeLower, bool includeUpper)
        {
            IFilter result = null;

            if (pricelists.IsNullOrEmpty())
            {
                result = CreateRangeFilter(commonFieldName, lower, upper, includeLower, includeUpper);
            }
            else if (pricelistNumber < pricelists.Count)
            {
                // Create negative query for previous pricelist
                IFilter previousPricelistQuery = null;
                if (pricelistNumber > 0)
                {
                    var previousFieldName = StringsHelper.JoinNonEmptyStrings("_", commonFieldName, pricelists[pricelistNumber - 1]).ToLowerInvariant();
                    previousPricelistQuery = CreateRangeFilter(previousFieldName, "0", null, false, false);
                }

                // Create positive query for current pricelist
                var currentFieldName = StringsHelper.JoinNonEmptyStrings("_", commonFieldName, pricelists[pricelistNumber]).ToLowerInvariant();
                var currentPricelistQuery = CreateRangeFilter(currentFieldName, lower, upper, includeLower, includeUpper);

                // Get query for next pricelist
                var nextPricelistQuery = GetPriceRangeFilterRecursive(pricelistNumber + 1, commonFieldName, pricelists, lower, upper, includeLower, includeUpper);

                result = previousPricelistQuery.Not().And(currentPricelistQuery.Or(nextPricelistQuery));
            }

            return result;
        }      
    }
}
