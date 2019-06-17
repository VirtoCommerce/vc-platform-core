using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class JsonExportProvider : IExportProvider
    {
        private readonly Stream _stream;

        public string TypeName => nameof(JsonExportProvider);
        public IExportProviderConfiguration Configuration { get; }
        public ExportedTypeMetadata Metadata { get; set; }

        private JsonSerializer Serializer { get; set; }
        private StreamWriter Writer { get; set; }


        public JsonExportProvider(Stream stream, IExportProviderConfiguration exportProviderConfiguration)
        {

            if (exportProviderConfiguration is JsonProviderConfiguration jsonProviderConfiguration)
            {
                Serializer = JsonSerializer.Create(jsonProviderConfiguration.Settings);
            }
            else
            {
                Serializer = JsonSerializer.CreateDefault();
            }

            _stream = stream;
            Configuration = exportProviderConfiguration;

            Writer = new StreamWriter(_stream);
        }

        public void WriteMetadata(ExportedTypeMetadata metadata)
        {

        }

        public void WriteRecord(object objectToRecord)
        {
            FilterProperties(objectToRecord);
            Serializer.Serialize(Writer, objectToRecord);
            Writer.Flush();
        }


        private void FilterProperties(object obj, string baseMemberName = null)
        {
            var type = obj.GetType();

            foreach (var property in type.GetProperties().Where(x => x.CanRead && x.CanWrite))
            {
                var propertyName = $"{baseMemberName}{(baseMemberName.IsNullOrEmpty() ? string.Empty : ".")}{property.Name}";
                var nestedType = GetNestedType(property.PropertyType);

                if (nestedType.IsSubclassOf(typeof(Entity)))
                {
                    if (!Metadata.PropertiesInfo.Any(x => x.Name.Contains($"{propertyName}.", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        property.SetValue(obj, null);
                    }
                    else
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                        {
                            var objectValues = property.GetValue(obj, null) as IEnumerable;
                            foreach (var value in objectValues)
                            {
                                FilterProperties(value, propertyName);
                            }
                        }
                    }
                }
                else if (!Metadata.PropertiesInfo.Any(x =>
                    x.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    property.SetValue(obj, null);
                }
            }
        }

        private Type GetNestedType(Type t)
        {
            var result = t;
            if (t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var definedGenericArgs = t.GetGenericArguments();
                if (definedGenericArgs.Any() && definedGenericArgs[0].IsSubclassOf(typeof(Entity)))
                {
                    result = definedGenericArgs[0];
                }
            }

            return result;
        }
    }
}
