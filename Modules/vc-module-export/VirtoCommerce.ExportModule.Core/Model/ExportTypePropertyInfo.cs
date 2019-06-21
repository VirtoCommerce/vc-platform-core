using System.Reflection;
using Newtonsoft.Json;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportTypePropertyInfo
    {
        public string Name { get; set; }

        [JsonIgnore]
        public MemberInfo MemberInfo { get; set; }

        public string ExportName { get; set; }

        public bool IsRequired { get; set; }

        public string DefaultValue { get; set; }
    }
}
