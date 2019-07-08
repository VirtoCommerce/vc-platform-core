using System;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportedTypeColumnInfo : ICloneable
    {
        public string Name { get; set; }

        public string Group { get; set; }

        public string ExportName { get; set; }

        public bool IsRequired { get; set; }

        public string DefaultValue { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
