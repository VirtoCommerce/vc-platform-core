using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class GeoSortInfo : SortInfo
    {
        private static readonly Regex _hasLocation = new Regex(@"(?<column>\w+)\((?<geopoint>-?\d+(?:\.\d+)?,\s*-?\d+(?:\.\d+)?)\)", RegexOptions.Compiled);

        public GeoPoint GeoPoint { get; set; }

        /// <summary>
        /// sort_field_name([-/+]dd.ddd,[+/-]dd.ddd)
        /// </summary>
        /// <param name="sortExpr"></param>
        /// <returns></returns>
        public static IEnumerable<SortInfo> TryParse(string sortExpr)
        {
            var result = new List<SortInfo>();

            if (!string.IsNullOrEmpty(sortExpr))
            {
                var sortInfoStrings = sortExpr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var sortInfoString in sortInfoStrings)
                {
                    var matches = _hasLocation.Matches(sortInfoString);
                    if (matches.Count > 0)
                    {
                        var geoSortInfo = new GeoSortInfo
                        {
                            SortColumn = matches[0].Groups["column"].Value,
                            GeoPoint = GeoPoint.TryParse(matches[0].Groups["geopoint"].Value),
                            SortDirection = sortInfoString.EndsWith("desc", StringComparison.InvariantCultureIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending
                        };
                        result.Add(geoSortInfo);
                    }
                    else
                    {
                        result.AddRange(Parse(sortInfoString));
                    }
                }
            }
            return result;
        }
     
        public override string ToString()
        {
            return $"{GeoPoint?.ToString()}:{ base.ToString() }";
        }

        public override bool Equals(object obj)
        {
            var result = base.Equals(obj);
            if (obj is GeoSortInfo other)
            {
                result = GeoPoint == other.GeoPoint;
            }
            return result;
        }

        public override int GetHashCode()
        {
            return GeoPoint != null ? GeoPoint.GetHashCode() : base.GetHashCode();
        }
    }
}
