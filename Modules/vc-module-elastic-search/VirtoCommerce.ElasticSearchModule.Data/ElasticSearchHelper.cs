using System.Globalization;
using Nest;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ElasticSearchModule.Data
{
    public static class ElasticSearchHelper
    {
        public static string ToElasticFieldName(string originalName)
        {
            return originalName?.ToLowerInvariant();
        }

        public static string ToStringInvariant(this object value)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", value);
        }

        public static object ToElasticValue(this GeoPoint point)
        {
            return point == null ? null : new { lat = point.Latitude, lon = point.Longitude };
        }

        public static GeoLocation ToGeoLocation(this GeoPoint point)
        {
            return point == null ? null : new GeoLocation(point.Latitude, point.Longitude);
        }
    }
}
