using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public sealed class JsonExportProvider : IExportProvider
    {
        public string TypeName => nameof(JsonExportProvider);
        public ExportedTypePropertyInfo[] IncludedProperties { get; private set; }
        public string ExportedFileExtension => "json";
        public bool IsTabular => false;
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

    public class ObjectDiscriminatorJsonConverter : JsonConverter
    {
        private readonly Type[] _types;
        private readonly JsonSerializer _jsonSerializer;

        public override bool CanRead => false;
        public override bool CanWrite => true;

        public ObjectDiscriminatorJsonConverter(JsonSerializerSettings jsonSerializerSettings, params Type[] types)
        {
            _types = types;
            _jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
        }

        public override bool CanConvert(Type objectType)
        {
            return _types.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Here we use another serializer with the same settings to avoid infinity cycle
            var jToken = JToken.FromObject(value, _jsonSerializer);

            if (jToken.Type == JTokenType.Object)
            {
                var jObject = (JObject)jToken;

                jObject.AddFirst(new JProperty("$discriminator", value.GetType().FullName));
                jObject.WriteTo(writer);
            }
            else
            {
                jToken.WriteTo(writer);
            }
        }
    }
}
