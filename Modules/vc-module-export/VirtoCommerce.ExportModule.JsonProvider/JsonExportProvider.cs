using System;
using System.IO;
using Newtonsoft.Json;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.JsonProvider
{
    public sealed class JsonExportProvider : IExportProvider
    {
        public string TypeName => nameof(JsonExportProvider);
        public ExportedTypePropertyInfo[] IncludedProperties { get; private set; }
        public string ExportedFileExtension => "json";
        public bool IsTabular => false;
        [JsonIgnore]
        // TODO: Temporary, before config rework - need to store only specific properties, not whole huge provider specific configs
        public IExportProviderConfiguration Configuration { get; }

        private readonly JsonSerializer _serializer;
        private JsonTextWriter _jsonTextWriter;

        public JsonExportProvider(ExportDataRequest exportDataRequest)
        {
            if (exportDataRequest == null)
            {
                throw new ArgumentNullException(nameof(exportDataRequest));
            }

            var jsonProviderConfiguration = exportDataRequest.ProviderConfig as JsonProviderConfiguration ?? new JsonProviderConfiguration();

            Configuration = jsonProviderConfiguration;
            IncludedProperties = exportDataRequest.DataQuery?.IncludedProperties;

            var jsonSettings = jsonProviderConfiguration.Settings;

            _serializer = JsonSerializer.Create(jsonSettings);

            _serializer.Converters.Add(new ObjectDiscriminatorJsonConverter(jsonSettings, typeof(Entity)));
        }

        public void WriteRecord(TextWriter writer, IExportable objectToRecord)
        {
            if (objectToRecord == null)
            {
                throw new ArgumentNullException(nameof(objectToRecord));
            }

            EnsureWriterCreated(writer);

            _serializer.Serialize(_jsonTextWriter, objectToRecord);
        }

        public void Dispose()
        {
            _jsonTextWriter?.WriteEndArray();
            _jsonTextWriter?.Flush();
            _jsonTextWriter?.Close();
        }


        private void EnsureWriterCreated(TextWriter writer)
        {
            if (_jsonTextWriter == null)
            {
                _jsonTextWriter = new JsonTextWriter(writer);
                _jsonTextWriter.CloseOutput = false;
                _jsonTextWriter.WriteStartArray();
            }
        }
    }
}
