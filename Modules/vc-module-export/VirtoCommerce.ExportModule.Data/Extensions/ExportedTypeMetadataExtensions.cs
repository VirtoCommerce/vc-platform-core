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
        /// <summary>
        /// Returns metadata for type plain (all except: Entity, IEnumerable &lt;Entity&gt;) properties.
        /// </summary>
        /// <param name="type">Type for getting metadata.</param>
        /// <returns>Metadata for the T type, including all non-reference properties of type.</returns>
        public static ExportedTypeMetadata GetPropertyNames(this Type type)
        {
            var result = new ExportedTypeMetadata
            {
                PropertyInfos = type.GetPropertyNames(type.Name).ToArray()
            };

            return result;
        }

        /// <summary>
        /// Returns metadata for type nested properties.
        /// </summary>
        /// <param name="type">Type for getting metadata.</param>
        /// <param name="propertyPaths">Property paths, e.g. PropertyB.PropertyC </param>
        /// <returns>Metadata with the nested property's property paths, e.g. [{ FullName: Id,  Group : 'PropertyB.PropertyC' }, ...] </returns>
        public static ExportedTypeMetadata GetNestedPropertyNames(this Type type, params string[] propertyPaths)
        {
            var result = new ExportedTypeMetadata
            {
                PropertyInfos = propertyPaths.SelectMany(x =>
                    type.GetPropertyType(x).GetPropertyNames(x))
                    .ToArray()
            };

            return result;
        }

        private static Type GetPropertyType(this Type type, string propertyPath)
        {
            return type.GetPropertyType(propertyPath.Split('.'));
        }

        private static Type GetPropertyType(this Type type, IEnumerable<string> propertyNames)
        {
            Type result;
            var nestedType = GetNestedType(type);
            if (propertyNames.Any())
            {
                result = nestedType.GetProperty(propertyNames.First()).PropertyType.GetPropertyType(propertyNames.Skip(1));
            }
            else
            {
                result = nestedType;
            }
            return result;
        }

        private static ExportedTypePropertyInfo[] GetPropertyNames(this Type type, string groupName)
        {
            var result = new List<ExportedTypePropertyInfo>();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead && x.CanWrite);

            foreach (var propertyInfo in properties)
            {
                var nestedType = GetNestedType(propertyInfo.PropertyType);
                var isNested = nestedType.IsSubclassOf(typeof(Entity));
                var memberName = propertyInfo.Name;

                if (!isNested)
                {
                    result.Add(new ExportedTypePropertyInfo()
                    {
                        FullName = memberName,
                        DisplayName = memberName,
                        Group = groupName,
                    });
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Adds baseName as a prefix to the property name (i.e. "{baseName}.{Name}")
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
