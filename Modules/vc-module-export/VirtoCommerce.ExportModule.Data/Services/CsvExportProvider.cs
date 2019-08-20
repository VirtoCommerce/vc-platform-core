using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Extensions;
using VirtoCommerce.ExportModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
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

    /// <summary>
    /// Custom ClassMap implementation which includes type properties. Supports nested properties.
    /// Does not map <see cref="IEnumerable<Entity>"/> as these are not representable in CSV structure in suitable manner.
    /// </summary>
    /// <typeparam name="T">Mapped type.</typeparam>
    public class MetadataFilteredMap<T> : ClassMap<T>
    {
        public MetadataFilteredMap() : this(null)
        { }

        public MetadataFilteredMap(ExportedTypePropertyInfo[] includedProperties)
        {
            var exportedType = typeof(T);
            var includedPropertiesInfo = includedProperties ?? exportedType.GetPropertyNames().PropertyInfos;
            var columnIndex = 0;

            foreach (var includedPropertyInfo in includedPropertiesInfo)
            {
                var propertyNames = includedPropertyInfo.FullName.Split('.');
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
                        var memberMap = CreateMemberMap(currentType, propertyInfo, includedPropertyInfo.DisplayName, ref columnIndex);

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

            return type.GetInterfaces().Any(x =>
                x.IsGenericType
                && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
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
