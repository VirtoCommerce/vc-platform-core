using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Basic query information for data sources to retrieve exported data: included columns, paging, sorting, etc...
    /// Applied data sources expand it by adding certain criteria (for example, additional information for searching)
    /// </summary>
    public abstract class ExportDataQuery : ValueObject
    {
        /// <summary>
        /// Keyword to search data
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// Object keys to search data
        /// </summary>
        public string[] ObjectIds { get; set; } = new string[] { };
        /// <summary>
        /// How to sort a data set matching a query
        /// </summary>
        public string Sort { get; set; }
        /// <summary>
        /// User selected columns to export 
        /// </summary>
        public ExportedTypeColumnInfo[] IncludedColumns { get; set; } = Array.Empty<ExportedTypeColumnInfo>();
        /// <summary>
        /// Paging: skip records
        /// </summary>
        public int? Skip { get; set; }
        /// <summary>
        /// Paging: records in one page
        /// </summary>
        public int? Take { get; set; } = 50;
    }
}
