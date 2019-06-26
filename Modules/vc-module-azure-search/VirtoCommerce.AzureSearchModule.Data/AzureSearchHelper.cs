using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Azure.Search.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.AzureSearchModule.Data
{
    public static class AzureSearchHelper
    {
        public const string FieldNamePrefix = "f_";
        public const string RawKeyFieldName = "__id";
        public const string KeyFieldName = FieldNamePrefix + RawKeyFieldName;
        public const string NonExistentFieldFilter = KeyFieldName + " eq ''";

        public static string ToAzureFieldName(string fieldName)
        {
            return !string.IsNullOrEmpty(fieldName) ? FieldNamePrefix + Regex.Replace(fieldName, @"\W", "_").ToLowerInvariant() : null;
        }

        public static string FromAzureFieldName(string azureFieldName)
        {
            return azureFieldName.StartsWith(FieldNamePrefix) ? azureFieldName.Substring(FieldNamePrefix.Length) : azureFieldName;
        }

        public static Field Get(this IList<Field> fields, string rawName)
        {
            var azureFieldName = ToAzureFieldName(rawName);
            return fields?.FirstOrDefault(f => f.Name.EqualsInvariant(azureFieldName));
        }

        public static string ToStringInvariant(this object value)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", value);
        }

        public static string GetGeoDistanceExpression(string azureFieldName, GeoPoint point)
        {
            var location = point.ToQueryValue();
            var result = $"geo.distance({azureFieldName}, {location})";
            return result;
        }

        public static string ToQueryValue(this GeoPoint point)
        {
            return string.Format(CultureInfo.InvariantCulture, "geography'POINT({0} {1})'", point.Longitude, point.Latitude);
        }

        public static object ToDocumentValue(this GeoPoint point)
        {
            return point == null ? null : new { type = "Point", coordinates = new[] { point.Longitude, point.Latitude } };
        }

        public static string JoinNonEmptyStrings(string separator, bool encloseInParenthesis, params string[] values)
        {
            var builder = new StringBuilder();
            var valuesCount = 0;

            foreach (var value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (valuesCount > 0)
                    {
                        builder.Append(separator);
                    }

                    builder.Append(value);
                    valuesCount++;
                }
            }

            if (valuesCount > 1 && encloseInParenthesis)
            {
                builder.Insert(0, "(");
                builder.Append(")");
            }

            return builder.ToString();
        }
    }
}
