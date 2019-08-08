using System;
using Newtonsoft.Json;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportedTypeDefinition
    {
        public string TypeName { get; set; }

        public string Group { get; set; }

        public ExportedTypeMetadata MetaData { get; set; }

        public ExportedTypeMetadata TabularMetaData { get; set; }

        public string ExportDataQueryType { get; set; }

        public bool IsTabularExportSupported { get => TabularDataConverter != null; }

        [JsonIgnore]
        public ITabularDataConverter TabularDataConverter { get; set; }

        [JsonIgnore]
        public Func<ExportDataQuery, IPagedDataSource> ExportedDataSourceFactory { get; set; }
    }
}
