namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportDataRequest
    {
        public string ExportTypeName { get; set; }
        public ExportDataQuery DataQuery { get; set; }
        public IExportProviderConfiguration ProviderConfig { get; set; }
        public string ProviderName { get; set; }
    }
}
