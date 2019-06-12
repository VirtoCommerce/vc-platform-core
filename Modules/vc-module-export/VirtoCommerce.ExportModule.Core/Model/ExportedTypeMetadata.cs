using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
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
            List<MemberInfo> passedNodes = new List<MemberInfo>();
            result.PropertiesInfo = result.GetFromType(t, string.Empty, passedNodes);
            return result;
        }

        private ExportTypePropertyInfo[] GetFromType(Type t, string baseMemberName, List<MemberInfo> passedNodes)
        {
            var result = new List<ExportTypePropertyInfo>();
            foreach (var pi in t.GetProperties().Where(x=>x.CanRead))
            {
                if (!passedNodes.Contains(pi))
                {
                    string derivedMemberName = $@"{baseMemberName}{(baseMemberName.IsNullOrEmpty() ? string.Empty : ".")}{pi.Name}";
                    Type nestType = GetNestType(pi.PropertyType);
                    if (nestType.IsSubclassOf(typeof(Entity)))
                    {
                        passedNodes.Add(pi);
                        result.AddRange(GetFromType(nestType, derivedMemberName, passedNodes));
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

        private Type GetNestType(Type t)
        {
            Type result = t;
            if (t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                Type[] definedGenericArgs = t.GetGenericArguments();
                if (definedGenericArgs.Any() && definedGenericArgs[0].IsSubclassOf(typeof(Entity)))
                {
                    result = definedGenericArgs[0];
                }
            }
            return result;
        }
    }
}
