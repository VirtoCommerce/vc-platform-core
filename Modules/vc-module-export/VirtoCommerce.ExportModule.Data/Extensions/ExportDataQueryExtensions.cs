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
            var includedColumns = dataQuery.IncludedColumns;

            foreach (var property in type.GetProperties().Where(x => x.CanRead && x.CanWrite))
            {
                var propertyName = ExportedTypeMetadata.GetDerivedName(baseMemberName, property);
                var nestedType = ExportedTypeMetadata.GetNestedType(property.PropertyType);

                if (nestedType.IsSubclassOf(typeof(Entity)))
                {
                    if (!includedColumns.Any(x => x.Name.StartsWith($"{propertyName}.", StringComparison.InvariantCultureIgnoreCase)))
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
                else if (!includedColumns.Any(x => x.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    property.SetValue(obj, null);
                }
            }
        }
    }
}
