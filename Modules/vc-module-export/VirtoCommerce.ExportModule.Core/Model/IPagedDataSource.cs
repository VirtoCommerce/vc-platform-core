using System.Collections.Generic;

namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Interface for implementing a data source iterator
    /// </summary>
    public interface IPagedDataSource
    {
        /// <summary>
        /// The number of the page that will returned by <see cref = "Fetch" />
        /// </summary>
        int CurrentPageNumber { get; }
        /// <summary>
        /// The number of records in the page
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// Specifies the offset for fetching records
        /// </summary>
        int? Skip { get; set; }
        /// <summary>
        /// Specifies the number of fetched records
        /// </summary>
        int? Take { get; set; }
        /// <summary>
        /// Gets total count of records for this data source
        /// </summary>
        /// <returns></returns>
        int GetTotalCount();
        /// <summary>
        /// Gets the data page from the source according to currently set <see cref = "CurrentPageNumber" /> and <see cref = "PageSize" />.
        /// Implementations should increment <see cref="CurrentPageNumber"/> after fetch.
        /// </summary>
        /// <returns>Is some data received</returns>
        bool Fetch();
        /// <summary>
        /// Property to get received data
        /// </summary>
        /// <returns>Exportable entities received after Fetch() </returns>
        IEnumerable<IExportable> Items { get; }
    }
}
