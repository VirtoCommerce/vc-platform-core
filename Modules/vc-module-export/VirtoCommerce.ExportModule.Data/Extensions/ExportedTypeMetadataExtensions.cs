using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Extensions
{
    public static class ExportedTypeMetadataExtensions
    {
        private class ExportTypePropertyInfoEx
        {
            public ExportedTypePropertyInfo ExportedPropertyInfo { get; set; }
            public bool IsReference { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }

        /// <summary>
        /// Returns metadata about exportable entity type.
        /// </summary>
        /// <typeparam name="T">Type for getting metadata.</typeparam>
        /// <param name="propertyPathsToInclude">Property full paths to include to metadata</param>
        /// <returns>Metadata for the T type, including all non-reference properties of types: T and corresponding to the passed properties.</returns>
        public static ExportedTypeMetadata GetPropertyNames(this Type type, params string[] propertyPathsToInclude)
        {
            var result = new ExportedTypeMetadata
            {
                PropertyInfos = GetPropertyNames(type, type.Name, string.Empty, propertyPathsToInclude)
                .Where(x => !x.IsReference)
                .Select(x => x.ExportedPropertyInfo)
                .ToArray()
            };

            return result;
        }

        private static Type GetPropertyType(Type type, string propertyName)
        {
            return GetPropertyType(type, propertyName.Split('.'));
        }

        private static Type GetPropertyType(Type type, IEnumerable<string> propertyNames)
        {
            Type result;
            var nestedType = GetNestedType(type);
            if (propertyNames.Any())
            {
                result = GetPropertyType(nestedType.GetProperty(propertyNames.First()).PropertyType, propertyNames.Skip(1));
            }
            else
            {
                result = nestedType;
            }
            return result;
        }

        private static ExportTypePropertyInfoEx[] GetPropertyNames(Type type, string groupName, string baseMemberName, string[] propertyPathsToInclude)
        {
            var result = new List<ExportTypePropertyInfoEx>();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead && x.CanWrite);

            foreach (var propertyInfo in properties)
            {
                var nestedType = GetNestedType(propertyInfo.PropertyType);
                var isNested = nestedType.IsSubclassOf(typeof(Entity));
                var memberName = propertyInfo.GetDerivedName(baseMemberName);

                if (!isNested)
                {
                    result.Add(new ExportTypePropertyInfoEx()
                    {
                        ExportedPropertyInfo = new ExportedTypePropertyInfo
                        {
                            FullName = memberName,
                            DisplayName = memberName,
                            Group = groupName,
                        },
                        PropertyInfo = propertyInfo,
                        IsReference = isNested,
                    });
                }
            }
            //Continue searching for members in every property path
            foreach (var propertyPathToInclude in propertyPathsToInclude)
            {
                result.AddRange(GetPropertyNames(
                    GetPropertyType(type, propertyPathToInclude),
                    string.Format($@"{groupName}.{propertyPathToInclude}"),
                    propertyPathToInclude,
                    new string[] { }));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Adds baseName as a prefixe to the property name (i.e. "{baseName}.{Name}")
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="baseName"></param>
        /// <returns></returns>
        public static string GetDerivedName(this PropertyInfo pi, string baseName) => $"{baseName}{(baseName.IsNullOrEmpty() ? string.Empty : ".")}{pi.Name}";

        /// <summary>
        /// Check if a type is IEnumerable<T> where T derived from <see cref="Entity"/>. If it is, returns T, otherwise source type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNestedType(this Type type)
        {
            var result = type;
            if (type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var definedGenericArgs = type.GetGenericArguments();
                if (definedGenericArgs.Any() && definedGenericArgs[0].IsSubclassOf(typeof(Entity)))
                {
                    result = definedGenericArgs[0];
                }
            }
            return result;
        }
    }
}
