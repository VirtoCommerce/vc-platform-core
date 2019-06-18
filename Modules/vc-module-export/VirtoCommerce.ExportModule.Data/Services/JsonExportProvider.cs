using System.IO;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public sealed class JsonExportProvider : IExportProvider
    {
        private readonly Stream _stream;

        public string TypeName => nameof(JsonExportProvider);
        public IExportProviderConfiguration Configuration { get; }
        public ExportedTypeMetadata Metadata { get; set; }

        public JsonExportProvider(Stream stream, IExportProviderConfiguration exportProviderConfiguration)
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

        public void Dispose()
        {
        }
    }
}
