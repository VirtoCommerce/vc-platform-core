using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public sealed class CsvExportProvider : IExportProvider
    {
        public string TypeName => nameof(CsvExportProvider);
        public IExportProviderConfiguration Configuration { get; }
        public ExportedTypeMetadata Metadata { get; set; }

        private readonly Stream _stream;

        private TextWriter _textWriter;
        private CsvWriter _csvWriter;

        public CsvExportProvider(Stream stream, IExportProviderConfiguration exportProviderConfiguration)
        {
            _stream = stream;
            Configuration = exportProviderConfiguration;
        }

        private void EnsureWriterCreated(Type objectType)
        {
            if (Metadata == null)
            {
                throw new ArgumentNullException(nameof(Metadata));
            }
            if (_csvWriter == null)
            {
                _textWriter = new StreamWriter(_stream, Encoding.UTF8, 1024, true) { AutoFlush = true };

                var csvConfiguration = (Configuration as CsvProviderConfiguration)?.Configuration ?? new Configuration();
                var mapForType = csvConfiguration.Maps[objectType];

                if (mapForType == null)
                {
                    var constructor = typeof(PlainFilteredMap<>).MakeGenericType(objectType).GetConstructor(new[] { typeof(ExportedTypeMetadata) });
                    var classMap = (ClassMap)constructor.Invoke(new[] { Metadata });
                    csvConfiguration.RegisterClassMap(classMap);
                }

                _csvWriter = new CsvWriter(_textWriter, csvConfiguration);
            }
        }

        public void WriteMetadata(ExportedTypeMetadata metadata)
        {
            throw new NotSupportedException($"{nameof(CsvExportProvider)} does not support writing metadata.");
        }

        public void WriteRecord(object objectToRecord)
        {
            EnsureWriterCreated(objectToRecord.GetType());

            _csvWriter.WriteRecords(new[] { objectToRecord });
            _csvWriter.Flush();
        }

        public void Dispose()
        {
            _csvWriter?.Dispose();
            _textWriter?.Dispose();
        }
    }

    public class PlainFilteredMap<T> : ClassMap<T>
    {
        public PlainFilteredMap(ExportedTypeMetadata exportedTypeMetadata)
        {
            var exportedType = typeof(T);

            var includedPropertiesInfo = exportedTypeMetadata.PropertiesInfo;
            int columnIndex = 0;

            foreach (var includedPropertyInfo in includedPropertiesInfo)
            {
                var propertyNames = includedPropertyInfo.Name.Split('.');
                var currentType = exportedType;
                ClassMap currentClassMap = this;

                for (int i = 0; i < propertyNames.Length; i++)
                {
                    var propertyName = propertyNames[i];
                    var propertyInfo = currentType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo?.CanRead != true)
                    {
                        break;
                    }

                    // Do not write enumerables (ICollection, etc.)
                    if (IsEnumerableEntityProperty(propertyInfo))
                    {
                        break;
                    }

                    // Add memberMap
                    if (i == propertyNames.Length - 1)
                    {
                        var memberMap = CreateMemberMap(currentType, propertyInfo, includedPropertyInfo.Name, ref columnIndex);

                        currentClassMap.MemberMaps.Add(memberMap);
                        currentClassMap = this;
                    }
                    // Working with nested properties - create or get References
                    else
                    {
                        var referenceMap = currentClassMap.ReferenceMaps.Find(propertyInfo);
                        currentType = propertyInfo.PropertyType;

                        if (referenceMap == null)
                        {
                            var referenceClassMapType = typeof(EmptyClassMapImpl<>).MakeGenericType(new[] { currentType });

                            referenceMap = new MemberReferenceMap(propertyInfo, (ClassMap)Activator.CreateInstance(referenceClassMapType));
                            currentClassMap.ReferenceMaps.Add(referenceMap);
                        }

                        currentClassMap = referenceMap.Data.Mapping;
                    }
                }
            }
        }

        private static bool IsEnumerableEntityProperty(PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;
            return type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                && type.GetGenericArguments().Any(x => x.IsSubclassOf(typeof(Entity)));
        }

        private static MemberMap CreateMemberMap(Type currentType, PropertyInfo propertyInfo, string columnName, ref int columnIndex)
        {
            var memberMap = MemberMap.CreateGeneric(currentType, propertyInfo);

            memberMap.Data.TypeConverterOptions.CultureInfo = CultureInfo.InvariantCulture;
            memberMap.Data.TypeConverterOptions.NumberStyle = NumberStyles.Any;
            memberMap.Data.TypeConverterOptions.BooleanTrueValues.AddRange(new List<string>() { "yes", "true" });
            memberMap.Data.TypeConverterOptions.BooleanFalseValues.AddRange(new List<string>() { "false", "no" });
            memberMap.Data.Names.Add(columnName);
            memberMap.Data.NameIndex = memberMap.Data.Names.Count - 1;
            memberMap.Data.Index = ++columnIndex;

            return memberMap;
        }
    }

    public class EmptyClassMapImpl<T> : ClassMap<T> { }
}
