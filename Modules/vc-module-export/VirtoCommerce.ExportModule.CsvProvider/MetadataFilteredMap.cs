using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CsvHelper.Configuration;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Extensions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.CsvProvider
{
    /// <summary>
    /// Custom ClassMap implementation which includes type properties. Supports nested properties.
    /// Does not map <see cref="IEnumerable<Entity>"/> as these are not representable in CSV structure in suitable manner.
    /// </summary>
    /// <typeparam name="T">Mapped type.</typeparam>
    public class MetadataFilteredMap<T> : ClassMap<T>
    {
        public MetadataFilteredMap() : this(null)
        { }

        public MetadataFilteredMap(ExportedTypePropertyInfo[] includedProperties)
        {
            var exportedType = typeof(T);
            var includedPropertiesInfo = includedProperties ?? exportedType.GetPropertyNames().PropertyInfos;
            var columnIndex = 0;

            foreach (var includedPropertyInfo in includedPropertiesInfo)
            {
                var propertyNames = includedPropertyInfo.FullName.Split('.');
                var currentType = exportedType;
                ClassMap currentClassMap = this;

                for (int i = 0; i < propertyNames.Length; i++)
                {
                    var propertyName = propertyNames[i];
                    var propertyInfo = currentType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo?.CanRead != true)
                    {
                        break;
                    }

                    // Do not write enumerables (ICollection, etc.)
                    if (IsEnumerableEntityProperty(propertyInfo))
                    {
                        break;
                    }

                    // Add memberMap
                    if (i == propertyNames.Length - 1)
                    {
                        var memberMap = CreateMemberMap(currentType, propertyInfo, includedPropertyInfo.DisplayName, ref columnIndex);

                        currentClassMap.MemberMaps.Add(memberMap);
                        currentClassMap = this;
                    }
                    // Working with nested properties - create or get References
                    else
                    {
                        var referenceMap = currentClassMap.ReferenceMaps.Find(propertyInfo);

                        currentType = propertyInfo.PropertyType;

                        if (referenceMap == null)
                        {
                            var referenceClassMapType = typeof(EmptyClassMapImpl<>).MakeGenericType(new[] { currentType });

                            referenceMap = new MemberReferenceMap(propertyInfo, (ClassMap)Activator.CreateInstance(referenceClassMapType));
                            currentClassMap.ReferenceMaps.Add(referenceMap);
                        }

                        currentClassMap = referenceMap.Data.Mapping;
                    }
                }
            }
        }


        private static bool IsEnumerableEntityProperty(PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;

            return type.GetInterfaces().Any(x =>
                x.IsGenericType
                && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                && type.GetGenericArguments().Any(x => x.IsSubclassOf(typeof(Entity)));
        }

        private static MemberMap CreateMemberMap(Type currentType, PropertyInfo propertyInfo, string columnName, ref int columnIndex)
        {
            var memberMap = MemberMap.CreateGeneric(currentType, propertyInfo);

            memberMap.Data.TypeConverterOptions.CultureInfo = CultureInfo.InvariantCulture;
            memberMap.Data.TypeConverterOptions.NumberStyle = NumberStyles.Any;
            memberMap.Data.TypeConverterOptions.BooleanTrueValues.AddRange(new List<string>() { "yes", "true" });
            memberMap.Data.TypeConverterOptions.BooleanFalseValues.AddRange(new List<string>() { "false", "no" });
            memberMap.Data.Names.Add(columnName);
            memberMap.Data.NameIndex = memberMap.Data.Names.Count - 1;
            memberMap.Data.Index = ++columnIndex;

            return memberMap;
        }
    }
}
