using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Queries;
using Lucene.Net.Search;
using Lucene.Net.Spatial.Queries;
using Lucene.Net.Spatial.Vector;
using Lucene.Net.Util;
using Spatial4n.Core.Context;
using Spatial4n.Core.Distance;
using VirtoCommerce.SearchModule.Core.Model;
using TermFilter = VirtoCommerce.SearchModule.Core.Model.TermFilter;

namespace VirtoCommerce.LuceneSearchModule.Data
{
    public class LuceneSearchFilterBuilder
    {
        public static Filter GetFilterRecursive(IFilter filter, ICollection<string> availableFields)
        {
            Filter result = null;

            var idsFilter = filter as IdsFilter;
            var termFilter = filter as TermFilter;
            var rangeFilter = filter as RangeFilter;
            var geoDistanceFilter = filter as GeoDistanceFilter;
            var notFilter = filter as NotFilter;
            var andFilter = filter as AndFilter;
            var orFilter = filter as OrFilter;

            if (idsFilter != null)
            {
                result = CreateIdsFilter(idsFilter);
            }
            else if (termFilter != null)
            {
                result = CreateTermFilter(termFilter, availableFields);
            }
            else if (rangeFilter != null)
            {
                result = CreateRangeFilter(rangeFilter);
            }
            else if (geoDistanceFilter != null)
            {
                result = CreateGeoDistanceFilter(geoDistanceFilter);
            }
            else if (notFilter != null)
            {
                result = CreateNotFilter(notFilter, availableFields);
            }
            else if (andFilter != null)
            {
                result = CreateAndFilter(andFilter, availableFields);
            }
            else if (orFilter != null)
            {
                result = CreateOrFilter(orFilter, availableFields);
            }

            return result;
        }


        private static Filter CreateIdsFilter(IdsFilter idsFilter)
        {
            Filter result = null;

            if (idsFilter?.Values != null)
            {
                result = CreateTermsFilter(LuceneSearchHelper.KeyFieldName, idsFilter.Values);
            }

            return result;
        }

        private static Filter CreateTermFilter(TermFilter termFilter, ICollection<string> availableFields)
        {
            Filter result = null;

            if (termFilter?.FieldName != null && termFilter.Values != null)
            {
                var isBooleanField = availableFields.Contains(LuceneSearchHelper.GetBooleanFieldName(termFilter.FieldName));
                var values = termFilter.Values.Select(v => GetFilterValue(v, isBooleanField)).ToArray();

                var fieldName = LuceneSearchHelper.ToLuceneFieldName(termFilter.FieldName);
                result = CreateTermsFilter(fieldName, values);
            }

            return result;
        }

        private static Filter CreateRangeFilter(RangeFilter rangeFilter)
        {
            Filter result = null;

            if (rangeFilter?.FieldName != null && rangeFilter.Values != null)
            {
                var fieldName = LuceneSearchHelper.ToLuceneFieldName(rangeFilter.FieldName);

                var childFilters = rangeFilter.Values.Select(v => CreateRangeFilterForValue(fieldName, v))
                    .Where(f => f != null)
                    .ToArray();

                result = JoinNonEmptyFilters(childFilters, Occur.SHOULD);
            }

            return result;
        }

        private static Filter CreateGeoDistanceFilter(GeoDistanceFilter geoDistanceFilter)
        {
            Filter result = null;

            if (geoDistanceFilter?.FieldName != null && geoDistanceFilter.Location != null)
            {
                var spatialContext = SpatialContext.GEO;
                var distance = DistanceUtils.Dist2Degrees(geoDistanceFilter.Distance, DistanceUtils.EARTH_MEAN_RADIUS_KM);
                var searchArea = spatialContext.MakeCircle(geoDistanceFilter.Location.Longitude, geoDistanceFilter.Location.Latitude, distance);
                var spatialArgs = new SpatialArgs(SpatialOperation.Intersects, searchArea);

                var fieldName = LuceneSearchHelper.ToLuceneFieldName(geoDistanceFilter.FieldName);
                var strategy = new PointVectorStrategy(spatialContext, fieldName);
                result = strategy.MakeFilter(spatialArgs);
            }

            return result;
        }

        private static Filter CreateNotFilter(NotFilter notFilter, ICollection<string> availableFields)
        {
            Filter result = null;

            var childFilter = GetFilterRecursive(notFilter.ChildFilter, availableFields);
            if (childFilter != null)
            {
                var booleanFilter = new BooleanFilter();
                booleanFilter.Add(new FilterClause(childFilter, Occur.MUST_NOT));
                result = booleanFilter;
            }

            return result;
        }

        private static Filter CreateAndFilter(AndFilter andFilter, ICollection<string> availableFields)
        {
            Filter result = null;

            if (andFilter?.ChildFilters != null)
            {
                var childFilters = andFilter.ChildFilters.Select(filter => GetFilterRecursive(filter, availableFields));
                result = JoinNonEmptyFilters(childFilters, Occur.MUST);
            }

            return result;
        }

        private static Filter CreateOrFilter(OrFilter orFilter, ICollection<string> availableFields)
        {
            Filter result = null;

            if (orFilter?.ChildFilters != null)
            {
                var childFilters = orFilter.ChildFilters.Select(filter => GetFilterRecursive(filter, availableFields));
                result = JoinNonEmptyFilters(childFilters, Occur.SHOULD);
            }

            return result;
        }

        public static Filter JoinNonEmptyFilters(IEnumerable<Filter> filters, Occur occur)
        {
            Filter result = null;

            if (filters != null)
            {
                var childFilters = filters.Where(f => f != null).ToArray();

                if (childFilters.Length > 1)
                {
                    var booleanFilter = new BooleanFilter();

                    foreach (var filter in childFilters)
                    {
                        booleanFilter.Add(new FilterClause(filter, occur));
                    }

                    result = booleanFilter;
                }
                else if (childFilters.Length > 0)
                {
                    result = childFilters.First();
                }
            }

            return result;
        }

        private static Filter CreateRangeFilterForValue(string fieldName, RangeFilterValue value)
        {
            return CreateRangeFilterForValue(fieldName, value.Lower, value.Upper, value.IncludeLower, value.IncludeUpper);
        }

        public static Filter CreateRangeFilterForValue(string fieldName, string lower, string upper, bool includeLower, bool includeUpper)
        {
            Filter result = null;

            // If both bounds are empty, ignore this range
            if (!string.IsNullOrEmpty(lower) || !string.IsNullOrEmpty(upper))
            {
                var lowerLong = ConvertToDateTimeTicks(lower);
                var upperLong = ConvertToDateTimeTicks(upper);
                if (lowerLong != null || upperLong != null)
                {
                    result = NumericRangeFilter.NewInt64Range(fieldName, lowerLong, upperLong, includeLower, includeUpper);
                }
                else
                {
                    var lowerDouble = ConvertToDouble(lower);
                    var upperDouble = ConvertToDouble(upper);
                    if (lowerDouble != null || upperDouble != null)
                    {
                        result = NumericRangeFilter.NewDoubleRange(fieldName, lowerDouble, upperDouble, includeLower, includeUpper);
                    }
                    else
                    {
                        result = TermRangeFilter.NewStringRange(fieldName, lower, upper, includeLower, includeUpper);
                    }
                }
            }

            return result;
        }

        public static Filter CreateTermsFilter(string fieldName, string value)
        {
            var query = new TermsFilter(new Term(fieldName, value));
            return query;
        }

        private static TermsFilter CreateTermsFilter(string fieldName, IEnumerable<string> values)
        {
            var query = new TermsFilter(values.Select(v => new Term(fieldName, v)).ToList());

            return query;
        }

        private static string GetFilterValue(string value, bool isBooleanField)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (isBooleanField)
            {
                return bool.Parse(value).ToStringInvariant();
            }

            var dateValue = ConvertToDateTimeTicks(value);
            if (dateValue != null)
            {
                return ConvertLongToString(dateValue.Value);
            }

            var dobuleValue = ConvertToDouble(value);
            if (dobuleValue.HasValue)
            {
                var longValue = NumericUtils.DoubleToSortableInt64(dobuleValue.Value);
                return ConvertLongToString(longValue);

            }
            return value;
        }

        private static long? ConvertToDateTimeTicks(string input)
        {
            long? result = null;

            DateTime value;
            if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
            {
                result = value.Ticks;
            }

            return result;
        }

        private static double? ConvertToDouble(string input)
        {
            double? result = null;

            double value;
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                result = value;
            }

            return result;
        }

        private static string ConvertLongToString(long longValue)
        {
            var act = new BytesRef();
            NumericUtils.Int64ToPrefixCoded(longValue, 0, act);
            return act.Utf8ToString();
        }
    }
}
