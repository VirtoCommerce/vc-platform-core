using System;
using Newtonsoft.Json;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportedTypeDefinition
    {
        public string TypeName { get; set; }

        public string Group { get; set; }

        public ExportedTypeMetadata MetaData { get; protected set; }

        public string ExportDataQueryType { get; set; }

        [JsonIgnore]
        public bool IsTabularExportSupported { get => TabularDataConverter != null; }

        [JsonIgnore]
        public ITabularDataConverter TabularDataConverter { get; protected set; }

        [JsonIgnore]
        public Func<ExportDataQuery, IPagedDataSource> ExportedDataSourceFactory { get; protected set; }

        public ExportedTypeDefinition WithDataSourceFactory(Func<ExportDataQuery, IPagedDataSource> factory)
        {
            ExportedDataSourceFactory = factory;
            return this;
        }

        public ExportedTypeDefinition WithMetadata(ExportedTypeMetadata metadata)
        {
            MetaData = metadata;
            return this;
        }

        public ExportedTypeDefinition WithTabularDataConverter(ITabularDataConverter tabularDataConverter)
        {
            TabularDataConverter = tabularDataConverter;
            return this;
        }
    }
}
