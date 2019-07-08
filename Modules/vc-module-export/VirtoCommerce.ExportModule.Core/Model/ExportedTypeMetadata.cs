using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportedTypeMetadata : ValueObject
    {
        private class ExportTypePropertyInfoEx
        {
            public ExportedTypeColumnInfo ColumnInfo { get; set; }
            public bool IsReference { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }

        public string Version { get; set; }
        public ExportedTypeColumnInfo[] PropertyInfos { get; set; }

        /// <summary>
        /// Returns basic info about potentially exported fields of entity.
        /// </summary>
        /// <typeparam name="T">Type for getting metadata.</typeparam>
        /// <param name="extractReferenceProperties">If true, reference properties properties (<see cref="Enitity"/> and collection of <see cref="Enitity"/>) will be extracted recursively.</param>
        /// <returns>Metadata for the T type.</returns>
        public static ExportedTypeMetadata GetFromType<T>(bool extractReferenceProperties)
        {
            var result = new ExportedTypeMetadata();
            var type = typeof(T);
            var passedNodes = new List<MemberInfo>();

            result.PropertyInfos = result.GetFromType(type, typeof(T).Name, string.Empty, passedNodes, extractReferenceProperties)
                .Where(x => !x.IsReference)
                .Select(x => x.ColumnInfo)
                .ToArray();

            return result;
        }


        public static string GetDerivedName(string baseName, PropertyInfo pi) => $"{baseName}{(baseName.IsNullOrEmpty() ? string.Empty : ".")}{pi.Name}";

        /// <summary>
        /// Check if a type is IEnumerable<T> where T derived from <see cref="Entity"/>. If it is, returns T, otherwise source type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNestedType(Type type)
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

        public override object Clone()
        {
            var result = (ExportedTypeMetadata)base.Clone();

            result.PropertyInfos = PropertyInfos?.Select(x => (ExportedTypeColumnInfo)x.Clone()).ToArray();

            return result;
        }


        private ExportTypePropertyInfoEx[] GetFromType(Type type, string groupName, string baseMemberName, List<MemberInfo> passedNodes, bool extractNestedProperties)
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

                    var memberName = GetDerivedName(baseMemberName, propertyInfo);

                    if (extractNestedProperties || !isNested)
                    {
                        result.Add(new ExportTypePropertyInfoEx()
                        {
                            ColumnInfo = new ExportedTypeColumnInfo
                            {
                                Name = memberName,
                                ExportName = memberName,
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
                result.AddRange(GetFromType(
                    nestedMemberInfo.NestedType,
                    groupName,
                    GetDerivedName(baseMemberName, (PropertyInfo)nestedMemberInfo.MemberInfo),
                    passedNodes,
                    extractNestedProperties));
            }

            return result.ToArray();
        }
    }
}
