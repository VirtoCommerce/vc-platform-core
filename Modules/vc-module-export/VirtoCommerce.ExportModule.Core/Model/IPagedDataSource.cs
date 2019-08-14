using System.Collections.Generic;

namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Interface for implementing a data source iterator
    /// </summary>
    public interface IPagedDataSource
    {
        /// <summary>
        /// The number of the page that will returned by <see cref = "FetchNextPage" />
        /// </summary>
        int CurrentPageNumber { get; }
        /// <summary>
        /// The number of records in the page
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// Get total count of records for this data source
        /// </summary>
        /// <returns></returns>
        int GetTotalCount();
        /// <summary>
        /// Get the data page from the source according to currently set <see cref = "CurrentPageNumber" /> and <see cref = "PageSize" />.
        /// Implementations should increment <see cref="CurrentPageNumber"/> after fetch.
        /// </summary>
        /// <returns>Exportable entities</returns>
        IEnumerable<IExportable> FetchNextPage();
    }
}
