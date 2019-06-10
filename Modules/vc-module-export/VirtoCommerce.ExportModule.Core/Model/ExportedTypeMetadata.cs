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
            result.PropertiesInfo = GetFromType(t, string.Empty);
            return result;
        }

        private static ExportTypePropertyInfo[] GetFromType(Type t, string baseMemberName)
        {
            var result = new List<ExportTypePropertyInfo>();
            foreach (var pi in t.GetProperties())
            {
                if (IsNested(pi.PropertyType))
                {
                    result.AddRange(GetFromType(pi.PropertyType, pi.Name));
                }
                else
                {
                    result.Add(new ExportTypePropertyInfo()
                    {
                        MemberInfo = pi,
                        Name = $@"{baseMemberName}{(baseMemberName.IsNullOrEmpty()?string.Empty:".")}{pi.Name}"
                    });
                }
            }
            return result.ToArray();
        }

        private static bool IsNested(Type t)
        {
            return t.IsSubclassOf(typeof(Entity));
        }
    }
}
