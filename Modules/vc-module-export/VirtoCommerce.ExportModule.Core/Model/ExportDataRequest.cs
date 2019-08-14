namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Incapsulates data required to start export: export type, query for data to export, provider to record
    /// </summary>
    public class ExportDataRequest
    {
        /// <summary>
        /// Full type name of exportable entity
        /// </summary>
        public string ExportTypeName { get; set; }
        /// <summary>
        /// Query information to retrive exported data
        /// </summary>
        public ExportDataQuery DataQuery { get; set; }
        public IExportProviderConfiguration ProviderConfig { get; set; }
        /// <summary>
        /// Type name of recording data provider
        /// </summary>
        public string ProviderName { get; set; }
    }
}
