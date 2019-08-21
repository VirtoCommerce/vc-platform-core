using System.Globalization;
using CsvHelper.Configuration;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.CsvProvider
{
    public class CsvProviderConfiguration : IExportProviderConfiguration
    {
        public Configuration Configuration { get; set; } = new Configuration(cultureInfo: CultureInfo.InvariantCulture);
    }
}
