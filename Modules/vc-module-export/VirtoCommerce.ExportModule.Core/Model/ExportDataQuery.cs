using System;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Basic query information for data sources to retrieve exported data: included properties, paging, sorting, etc...
    /// Applied data sources expand it by adding certain criteria (for example, additional information for searching)
    /// </summary>
    public abstract class ExportDataQuery : ValueObject
    {
        /// <summary>
        /// This used to instantiate a data query of this type at export start.
        /// </summary>
        [JsonProperty("exportTypeName")]
        public string ExportTypeName => GetType().Name;
        /// <summary>
        /// Keyword to search data
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// Object keys to search data
        /// </summary>
        public string[] ObjectIds { get; set; } = new string[] { };
        /// <summary>
        /// How to sort the dataset matching a query
        /// </summary>
        public string Sort { get; set; }
        /// <summary>
        /// User selected properties to export 
        /// </summary>
        public ExportedTypePropertyInfo[] IncludedProperties { get; set; } = Array.Empty<ExportedTypePropertyInfo>();
        /// <summary>
        /// Paging: skip records
        /// </summary>
        public int? Skip { get; set; }
        /// <summary>
        /// Paging: records in one page
        /// </summary>
        public int? Take { get; set; }
    }
}
