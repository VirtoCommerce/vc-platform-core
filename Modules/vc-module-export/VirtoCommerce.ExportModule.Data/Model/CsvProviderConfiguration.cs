using CsvHelper.Configuration;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Data.Model
{
    public class CsvProviderConfiguration : IExportProviderConfiguration
    {
        public Configuration Configuration { get; set; }
    }
}
