using System;
using Newtonsoft.Json;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Definition of exported entity type
    /// </summary>
    public class ExportedTypeDefinition
    {
        /// <summary>
        /// Logical type name, given during registration. It could be non-equal to exportable type name.
        /// Users can see it in a type selector blade.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Logical group name. Definitions with the same group names fall into one folder in type selector blade.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Metadata for this definition (set of properties, version)
        /// </summary>
        public ExportedTypeMetadata MetaData { get; set; }

        /// <summary>
        /// Metadata for this definition in case of supported tabular export (flat files, like *.csv).
        /// </summary>
        public ExportedTypeMetadata TabularMetaData { get; set; }

        /// <summary>
        /// Type name for <see cref="ExportDataQuery"/> descendand. This used to instantiate data query of this type at export start.
        /// </summary>
        public string ExportDataQueryType { get; set; }

        /// <summary>
        /// Returns <see cref="true"/> if tabular export supported, <see cref="TabularDataConverter"/> is set. 
        /// </summary>
        public bool IsTabularExportSupported { get => TabularDataConverter != null; }

        /// <summary>
        /// Converter for transforming exportable entity to tabular representation
        /// </summary>
        [JsonIgnore]
        public ITabularDataConverter TabularDataConverter { get; set; }

        /// <summary>
        /// Factory function to create a data source for this type
        /// </summary>
        [JsonIgnore]
        public Func<ExportDataQuery, IPagedDataSource> ExportedDataSourceFactory { get; set; }
    }
}
