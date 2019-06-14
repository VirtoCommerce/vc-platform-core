using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportedTypeMetadata : ValueObject
    {

        public string Version { get; set; }
        public ExportTypePropertyInfo[] PropertiesInfo { get; set; }

        public static ExportedTypeMetadata GetFromType<T>()
        {
            var result = new ExportedTypeMetadata();
            var t = typeof(T);
            var passedNodes = new List<MemberInfo>();
            result.PropertiesInfo = result.GetFromType(t, string.Empty, passedNodes);
            return result;
        }

        private ExportTypePropertyInfo[] GetFromType(Type t, string baseMemberName, List<MemberInfo> passedNodes)
        {
            var result = new List<ExportTypePropertyInfo>();
            foreach (var pi in t.GetProperties().Where(x => x.CanRead))
            {
                if (!passedNodes.Contains(pi))
                {
                    var derivedMemberName = $"{baseMemberName}{(baseMemberName.IsNullOrEmpty() ? string.Empty : ".")}{pi.Name}";
                    var nestedType = GetNestedType(pi.PropertyType);
                    if (nestedType.IsSubclassOf(typeof(Entity)))
                    {
                        passedNodes.Add(pi);
                        result.AddRange(GetFromType(nestedType, derivedMemberName, passedNodes));
                    }
                    else
                    {
                        result.Add(new ExportTypePropertyInfo()
                        {
                            MemberInfo = pi,
                            Name = derivedMemberName
                        });
                    }
                }
            }
            return result.ToArray();
        }

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
