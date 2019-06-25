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

            if (!includedPropertiesInfo.IsNullOrEmpty())
            {
                var includedMembers = new HashSet<MemberInfo>(includedPropertiesInfo.Select(x => x.MemberInfo));
                var exportedTypeProperties = exportedType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead && x.GetMethod.IsPublic).ToArray();

                foreach (var exportedTypeProperty in exportedTypeProperties)
                {
                    if (includedMembers.Contains(exportedTypeProperty))
                    {
                        var memberMap = MemberMap.CreateGeneric(exportedType, exportedTypeProperty);

                        memberMap.Data.TypeConverterOptions.CultureInfo = CultureInfo.InvariantCulture;
                        memberMap.Data.TypeConverterOptions.NumberStyle = NumberStyles.Any;
                        memberMap.Data.TypeConverterOptions.BooleanTrueValues.AddRange(new List<string>() { "yes", "true" });
                        memberMap.Data.TypeConverterOptions.BooleanFalseValues.AddRange(new List<string>() { "false", "no" });

                        MemberMaps.Add(memberMap);
                    }
                }
            }
        }
    }
}
