using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportTypePropertyInfo
    {
        public string Name { get; set; }
        public MemberInfo MemberInfo { get; set; }
        public string ExportName { get; set; }
        public bool IsRequired { get; set; }
        public string DefaultValue { get; set; }
    }
}
