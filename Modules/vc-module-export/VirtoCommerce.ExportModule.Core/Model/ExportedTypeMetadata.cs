using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportedTypeMetadata : ValueObject
    {
        /// <summary>
        /// Extended type info. Stores if specific property is entity (true) or not (false)
        /// </summary>
        private class ExportTypePropertyInfoEx
        {
            public ExportTypePropertyInfo PropertyInfo { get; set; }
            public bool IsEntity { get; set; }
        }

        public string Version { get; set; }
        public ExportTypePropertyInfo[] PropertiesInfo { get; set; }

        /// <summary>
        /// Returns basic info about potentially exported fields of entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ExportedTypeMetadata GetFromType<T>()
        {
            var result = new ExportedTypeMetadata();
            var type = typeof(T);
            var passedNodes = new List<MemberInfo>();
            result.PropertiesInfo = result.GetFromType(type, string.Empty, passedNodes).Where(x => !x.IsEntity).Select(x => x.PropertyInfo).ToArray();
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

        public ExportedTypeMetadata MakeShallowCopy()
        {
            return (ExportedTypeMetadata)MemberwiseClone();
        }


        private ExportTypePropertyInfoEx[] GetFromType(Type type, string baseMemberName, List<MemberInfo> passedNodes)
        {
            var result = new List<ExportTypePropertyInfoEx>();
            var nestedMemberInfos = new List<(MemberInfo MemberInfo, Type NestedType)>();

            foreach (var propertyInfo in type.GetProperties().Where(x => x.CanRead))
            {
                if (!passedNodes.Contains(propertyInfo))
                {
                    var nestedType = GetNestedType(propertyInfo.PropertyType);
                    var isEntity = false;

                    if (nestedType.IsSubclassOf(typeof(Entity)))
                    {
                        isEntity = true;
                        // Collect nested members for later inspection after all properties in this type
                        nestedMemberInfos.Add((propertyInfo, nestedType));
                    }

                    passedNodes.Add(propertyInfo);

                    var memberName = GetDerivedName(baseMemberName, propertyInfo);

                    result.Add(new ExportTypePropertyInfoEx()
                    {
                        PropertyInfo = new ExportTypePropertyInfo()
                        {
                            MemberInfo = propertyInfo,
                            Name = memberName,
                            ExportName = memberName,
                        },
                        IsEntity = isEntity
                    });
                }
            }

            foreach (var nestedMemberInfo in nestedMemberInfos)
            {
                //Continue searching for nested members
                result.AddRange(GetFromType(
                    nestedMemberInfo.NestedType,
                    GetDerivedName(baseMemberName, (PropertyInfo)nestedMemberInfo.MemberInfo),
                    passedNodes));
            }

            return result.ToArray();
        }
    }
}
