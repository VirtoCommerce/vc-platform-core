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
            var t = typeof(T);
            var passedNodes = new List<MemberInfo>();
            result.PropertiesInfo = result.GetFromType(t, string.Empty, passedNodes).Where(x => !x.IsEntity).Select(x => x.PropertyInfo).ToArray();
            return result;
        }

        private ExportTypePropertyInfoEx[] GetFromType(Type t, string baseMemberName, List<MemberInfo> passedNodes)
        {
            var result = new List<ExportTypePropertyInfoEx>();
            var membersForNodes = new List<(MemberInfo MemberInfo, Type NestedType)>();

            foreach (var pi in t.GetProperties().Where(x => x.CanRead))
            {
                if (!passedNodes.Contains(pi))
                {
                    var nestedType = GetNestedType(pi.PropertyType);
                    bool isEntity = false;
                    if (nestedType.IsSubclassOf(typeof(Entity)))
                    {
                        isEntity = true;
                        // Collect nested members for later inspection after all properties in this type
                        membersForNodes.Add((pi, nestedType));
                    }
                    passedNodes.Add(pi);
                    result.Add(new ExportTypePropertyInfoEx()
                    {
                        PropertyInfo = new ExportTypePropertyInfo()
                        {
                            MemberInfo = pi,
                            Name = $"{baseMemberName}{(baseMemberName.IsNullOrEmpty() ? string.Empty : ".")}{pi.Name}"
                        },
                        IsEntity = isEntity
                    });
                }
            }

            foreach (var pi in membersForNodes)
            {
                //Continue searching for nested members
                result.AddRange(GetFromType(pi.NestedType, $"{baseMemberName}{(baseMemberName.IsNullOrEmpty() ? string.Empty : ".")}{pi.MemberInfo.Name}", passedNodes));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Check if a type is IEnumerable<T> where T derived from <see cref="Entity"/>. If it is, returns T, otherwise t.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Type GetNestedType(Type t)
        {
            var result = t;
            if (t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var definedGenericArgs = t.GetGenericArguments();
                if (definedGenericArgs.Any() && definedGenericArgs[0].IsSubclassOf(typeof(Entity)))
                {
                    result = definedGenericArgs[0];
                }
            }
            return result;
        }
    }
}
