using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Core.Services
{
    /// <summary>
    /// Used by indexing manager to find objects to be indexed
    /// </summary>
    public interface IIndexDocumentChangesProvider
    {
        /// <summary>
        /// Returns total count of the changes in the given time interval. If both startDate and endDate are null, returns total count of all available objects.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Returns IDs of objects changed in the given time interval. If both startDate and endDate are null, returns IDs of all available objects.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take);
    }
}
