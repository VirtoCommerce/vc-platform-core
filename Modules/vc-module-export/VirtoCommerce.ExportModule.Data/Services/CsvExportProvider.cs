using System.IO;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class CsvExportProvider : IExportProvider
    {
        private readonly Stream _stream;

        public string TypeName => nameof(CsvExportProvider);
        public IExportProviderConfiguration Configuration { get; }
        public ExportedTypeMetadata Metadata { get; set; }

        public CsvExportProvider(Stream stream, IExportProviderConfiguration exportProviderConfiguration)
        {
            _stream = stream;
            Configuration = exportProviderConfiguration;
        }

        public void WriteMetadata(ExportedTypeMetadata metadata)
        {
            throw new System.NotImplementedException();
        }

        public void WriteRecord(object objectToRecord)
        {
            throw new System.NotImplementedException();
        }
    }
}
