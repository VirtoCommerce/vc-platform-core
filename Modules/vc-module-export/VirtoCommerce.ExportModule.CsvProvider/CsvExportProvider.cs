using System;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Model;

namespace VirtoCommerce.ExportModule.CsvProvider
{
    public sealed class CsvExportProvider : IExportProvider
    {
        public string TypeName => nameof(CsvExportProvider);
        public ExportedTypePropertyInfo[] IncludedProperties { get; private set; }
        public string ExportedFileExtension => "csv";
        public bool IsTabular => true;
        public IExportProviderConfiguration Configuration { get; }

        private CsvWriter _csvWriter;

        public CsvExportProvider(ExportDataRequest exportDataRequest)
        {
            if (exportDataRequest == null)
            {
                throw new ArgumentNullException(nameof(exportDataRequest));
            }

            Configuration = exportDataRequest.ProviderConfig;
            IncludedProperties = exportDataRequest.DataQuery?.IncludedProperties;
        }

        public void WriteRecord(TextWriter writer, IExportable objectToRecord)
        {
            EnsureWriterCreated(writer);

            AddClassMap(objectToRecord.GetType());

            _csvWriter.WriteRecords(new object[] { objectToRecord });
        }

        public void Dispose()
        {
            _csvWriter?.Flush();
            _csvWriter?.Dispose();
        }


        private void EnsureWriterCreated(TextWriter textWriter)
        {
            if (_csvWriter == null)
            {
                var csvConfiguration = (Configuration as CsvProviderConfiguration)?.Configuration ?? new Configuration();

                _csvWriter = new CsvWriter(textWriter, csvConfiguration, true);
            }
        }

        private void AddClassMap(Type objectType)
        {
            var csvConfiguration = _csvWriter.Configuration;
            var mapForType = csvConfiguration.Maps[objectType];

            if (mapForType == null)
            {
                var constructor = typeof(MetadataFilteredMap<>).MakeGenericType(objectType).GetConstructor(IncludedProperties != null
                    ? new[] { typeof(ExportedTypePropertyInfo[]) }
                    : Array.Empty<Type>());
                var classMap = (ClassMap)constructor.Invoke(IncludedProperties != null ? new[] { IncludedProperties } : null);

                csvConfiguration.RegisterClassMap(classMap);
            }
        }
    }
}
