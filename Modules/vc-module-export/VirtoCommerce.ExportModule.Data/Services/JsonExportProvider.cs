using System;
using System.Collections;
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
        public string ExportedFileExtension => "json";
        public bool IsTabular => false;
        public IExportProviderConfiguration Configuration { get; }
        public ExportedTypeMetadata Metadata { get; set; }

        private readonly JsonSerializer _serializer;
        private JsonTextWriter _jsonTextWriter;

        public JsonExportProvider(IExportProviderConfiguration exportProviderConfiguration)
        {
            Configuration = exportProviderConfiguration;

            if (exportProviderConfiguration is JsonProviderConfiguration jsonProviderConfiguration)
            {
                _serializer = JsonSerializer.Create(jsonProviderConfiguration.Settings);
            }
            else
            {
#if DEBUG
                _serializer = JsonSerializer.Create(new JsonSerializerSettings() { Formatting = Formatting.Indented });
#else
                _serializer = JsonSerializer.CreateDefault();
#endif
            }

            _serializer.Converters.Add(new ObjectDiscriminatorJsonConverter(typeof(Entity)));
        }

        public void WriteRecord(TextWriter writer, object objectToRecord)
        {
            if (objectToRecord == null)
            {
                throw new ArgumentNullException(nameof(objectToRecord));
            }

            EnsureWriterCreated(writer);
            FilterProperties(objectToRecord);

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

        private void FilterProperties(object obj, string baseMemberName = null)
        {
            var type = obj.GetType();

            foreach (var property in type.GetProperties().Where(x => x.CanRead && x.CanWrite))
            {
                var propertyName = ExportedTypeMetadata.GetDerivedName(baseMemberName, property);
                var nestedType = ExportedTypeMetadata.GetNestedType(property.PropertyType);

                if (nestedType.IsSubclassOf(typeof(Entity)))
                {
                    if (!Metadata.PropertyInfos.Any(x => x.Name.StartsWith($"{propertyName}.", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        property.SetValue(obj, null);
                    }
                    else
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                        {
                            var objectValues = property.GetValue(obj, null) as IEnumerable;
                            if (objectValues != null)
                            {
                                foreach (var value in objectValues)
                                {
                                    FilterProperties(value, propertyName);
                                }
                            }
                        }
                        else
                        {
                            var objectValue = property.GetValue(obj, null);
                            if (objectValue != null)
                            {
                                FilterProperties(objectValue, propertyName);
                            }
                        }
                    }
                }
                else if (!Metadata.PropertyInfos.Any(x => x.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    property.SetValue(obj, null);
                }
            }
        }
    }

    public class ObjectDiscriminatorJsonConverter : JsonConverter
    {
        private readonly Type[] _types;

        public override bool CanRead => false;
        public override bool CanWrite => true;

        public ObjectDiscriminatorJsonConverter(params Type[] types)
        {
            _types = types;
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
            var jToken = JToken.FromObject(value);

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
