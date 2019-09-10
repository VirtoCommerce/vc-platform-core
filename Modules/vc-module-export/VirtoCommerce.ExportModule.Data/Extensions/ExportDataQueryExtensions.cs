using System;
using System.Collections;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Extensions
{
    public static class ExportDataQueryExtensions
    {
        public static void FilterProperties(this ExportDataQuery dataQuery, object obj, string baseMemberName = null)
        {
            var type = obj.GetType();
            var includedProperties = dataQuery.IncludedProperties;

            if (!includedProperties.IsNullOrEmpty())
            {
                foreach (var property in type.GetProperties().Where(x => x.CanRead && x.CanWrite))
                {
                    var propertyName = property.GetDerivedName(baseMemberName);

                    if (property.PropertyType.IsNested())
                    {
                        if (!includedProperties.Any(x => x.FullName.StartsWith($"{propertyName}.", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            property.SetValue(obj, null);
                        }
                        else
                        {
                            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                            {
                                var objectValues = property.GetValue(obj, null) as IEnumerable;
                                if (objectValues != null)
                                {
                                    foreach (var value in objectValues)
                                    {
                                        FilterProperties(dataQuery, value, propertyName);
                                    }
                                }
                            }
                            else
                            {
                                var objectValue = property.GetValue(obj, null);
                                if (objectValue != null)
                                {
                                    FilterProperties(dataQuery, objectValue, propertyName);
                                }
                            }
                        }
                    }
                    else if (!includedProperties.Any(x => x.FullName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        property.SetValue(obj, null);
                    }
                }
            }
        }
    }
}
