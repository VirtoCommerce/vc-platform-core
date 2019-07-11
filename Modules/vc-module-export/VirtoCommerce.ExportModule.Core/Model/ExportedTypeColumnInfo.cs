namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportedTypeColumnInfo
    {
        public string Name { get; set; }

        public string Group { get; set; }

        public string ExportName { get; set; }

        public bool IsRequired { get; set; }

        public string DefaultValue { get; set; }
    }
}
