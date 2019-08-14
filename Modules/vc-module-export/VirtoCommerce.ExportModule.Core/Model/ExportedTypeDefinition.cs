using System;
using Newtonsoft.Json;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Definition of single exported entity type
    /// </summary>
    public class ExportedTypeDefinition
    {
        /// <summary>
        /// Logical type name, given during registration. Commonly non-equal to <see cref="ExportableEntity{T}"/> descendants type names.
        /// Users can see it in type selector blade.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Logical group name. Definitions with the same group names fall into the folder in type selector blade.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Metadata for this definition (set of columns, version)
        /// </summary>
        public ExportedTypeMetadata MetaData { get; set; }

        /// <summary>
        /// Metadata for this definition in the case of supported tabular export (flat files, like *.csv).
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
        /// Converter for transforming <see cref="ExportableEntity{T}"/> to tabular form
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
