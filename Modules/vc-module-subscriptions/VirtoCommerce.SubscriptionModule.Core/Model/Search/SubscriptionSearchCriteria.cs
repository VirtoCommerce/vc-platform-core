using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SubscriptionModule.Core.Model.Search
{
    public class SubscriptionSearchCriteria : SearchCriteriaBase
    {
        /// <summary>
        /// Search within specified store
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// Search by subscription number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Search subscription for related order id
        /// </summary>
        public string CustomerOrderId { get; set; }
        /// <summary>
        /// Search subscription in StartDate and EndDate range inclusive
        /// </summary>
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? ModifiedSinceDate { get; set; }

        public string CustomerId { get; set; }
        /// <summary>
        /// Search with specified statuses
        /// </summary>
        public string[] Statuses { get; set; }

        /// <summary>
        /// Search by external subscription system id
        /// </summary>
        public string OuterId { get; set; }
    }
}
