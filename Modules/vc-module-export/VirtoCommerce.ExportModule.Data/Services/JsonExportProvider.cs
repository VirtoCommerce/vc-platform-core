using System;
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public sealed class JsonExportProvider : IExportProvider
    {
        private readonly Stream _stream;

        public string TypeName => nameof(JsonExportProvider);
        public IExportProviderConfiguration Configuration { get; }
        public ExportedTypeMetadata Metadata { get; set; }

        private readonly JsonSerializer _serializer;
        private StreamWriter _streamWriter;


        public JsonExportProvider(Stream stream, IExportProviderConfiguration exportProviderConfiguration)
        {
            if (exportProviderConfiguration is JsonProviderConfiguration jsonProviderConfiguration)
            {
                _serializer = JsonSerializer.Create(jsonProviderConfiguration.Settings);
            }
            else
            {
                _serializer = JsonSerializer.CreateDefault();
            }

            _stream = stream;
            Configuration = exportProviderConfiguration;
        }

        public void WriteMetadata(ExportedTypeMetadata metadata)
        {
        }

        public void WriteRecord(object objectToRecord)
        {
            EnsureWriterCreated();
            FilterProperties(objectToRecord);
            _serializer.Serialize(_streamWriter, objectToRecord);
            _streamWriter.Flush();
        }


        private void EnsureWriterCreated()
        {
            if (_streamWriter == null)
            {
                _streamWriter = new StreamWriter(_stream);
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
                    if (!Metadata.PropertiesInfo.Any(x => x.Name.StartsWith($"{propertyName}.", StringComparison.InvariantCultureIgnoreCase)))
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
                else if (!Metadata.PropertiesInfo.Any(x => x.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    property.SetValue(obj, null);
                }
            }
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }
    }
}
