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
            public ExportedTypePropertyInfo ColumnInfo { get; set; }
            public bool IsReference { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }

        /// <summary>
        /// Returns basic info about potentially exported fields of entity.
        /// </summary>
        /// <typeparam name="T">Type for getting metadata.</typeparam>
        /// <param name="extractReferenceProperties">If true, reference properties properties (<see cref="Enitity"/> and collection of <see cref="Enitity"/>) will be extracted recursively.</param>
        /// <returns>Metadata for the T type.</returns>
        public static ExportedTypeMetadata GetPropertyNames(this Type type, bool extractReferenceProperties)
        {
            var result = new ExportedTypeMetadata();
            var passedNodes = new List<MemberInfo>();

            result.PropertyInfos = GetPropertyNames(type, type.Name, string.Empty, passedNodes, extractReferenceProperties)
                .Where(x => !x.IsReference)
                .Select(x => x.ColumnInfo)
                .ToArray();

            return result;
        }

        private static ExportTypePropertyInfoEx[] GetPropertyNames(Type type, string groupName, string baseMemberName, List<MemberInfo> passedNodes, bool extractNestedProperties)
        {
            var result = new List<ExportTypePropertyInfoEx>();
            var nestedMemberInfos = new List<(MemberInfo MemberInfo, Type NestedType)>();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead && x.CanWrite);

            foreach (var propertyInfo in properties)
            {
                if (!passedNodes.Contains(propertyInfo))
                {
                    var nestedType = GetNestedType(propertyInfo.PropertyType);
                    var isNested = nestedType.IsSubclassOf(typeof(Entity));

                    // Collect nested members for later inspection after all properties in this type
                    if (extractNestedProperties && isNested)
                    {
                        nestedMemberInfos.Add((propertyInfo, nestedType));
                    }

                    passedNodes.Add(propertyInfo);

                    var memberName = propertyInfo.GetDerivedName(baseMemberName);

                    if (extractNestedProperties || !isNested)
                    {
                        result.Add(new ExportTypePropertyInfoEx()
                        {
                            ColumnInfo = new ExportedTypePropertyInfo
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
            }

            foreach (var nestedMemberInfo in nestedMemberInfos)
            {
                //Continue searching for nested members
                result.AddRange(GetPropertyNames(
                    nestedMemberInfo.NestedType,
                    string.Format($@"{groupName}.{nestedMemberInfo.MemberInfo.Name}"),
                    ((PropertyInfo)nestedMemberInfo.MemberInfo).GetDerivedName(baseMemberName),
                    passedNodes,
                    extractNestedProperties));
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
