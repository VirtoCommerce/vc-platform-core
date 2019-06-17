using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.PricingModule.Core.Services
{
    /// <summary>
    /// Service responsible for providing changes for the prices that are not caused by user changes.
    /// </summary>
    public interface IPricingChangesService
    {
        /// <summary>
        /// Returns all price changes due to date filtering.
        /// </summary>
        /// <param name="lastEvaluationTimestamp">Last time this evaluation was called. 
        /// If this parameter is null, beginning of time will be used.</param>
        /// <param name="evaluationTimestamp">Moment to evaluate. If this parameter is null, 
        /// current UTC time will be used.</param>
        /// <param name="skip">Optional count of price changes to skip for the pagination.</param>
        /// <param name="take">Optional count of price changes to take for the pagination.</param>
        /// <returns></returns>
        Task<IList<IndexDocumentChange>> GetCalendarChangesAsync(DateTime? lastEvaluationTimestamp,
            DateTime? evaluationTimestamp, int skip, int take);
    }
}
