using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Core.Model.Search
{
    public class CustomerOrderSearchCriteria : SearchCriteriaBase
    {
        public string[] Ids { get; set; }
        /// <summary>
        /// Search by numbers
        /// </summary>
        public string Number { get; set; }

        private string[] _numbers;
        public string[] Numbers
        {
            get
            {
                if (_numbers == null && !string.IsNullOrEmpty(Number))
                {
                    _numbers = new[] { Number };
                }
                return _numbers;
            }
            set
            {
                _numbers = value;
            }
        }

        /// <summary>
        /// Search orders with flag IsPrototype
        /// </summary>
        public bool WithPrototypes { get; set; }

        /// <summary>
        /// Search only recurring orders created by subscription
        /// </summary>
        public bool OnlyRecurring { get; set; }

        /// <summary>
        /// Search orders with given subscription
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Search orders with given subscriptions
        /// </summary>
        private string[] _subscriptionIds;
        public string[] SubscriptionIds
        {
            get
            {
                if (_subscriptionIds == null && !string.IsNullOrEmpty(SubscriptionId))
                {
                    _subscriptionIds = new[] { SubscriptionId };
                }
                return _subscriptionIds;
            }
            set
            {
                _subscriptionIds = value;
            }
        }

        /// <summary>
        /// Search by status
        /// </summary>
        public string Status { get; set; }

        private string[] _statuses;
        public string[] Statuses
        {
            get
            {
                if (_statuses == null && !string.IsNullOrEmpty(Status))
                {
                    _statuses = new[] { Status };
                }
                return _statuses;
            }
            set
            {
                _statuses = value;
            }
        }


        /// <summary>
        /// It used to limit search within an operation (customer order for example)
        /// </summary>
        public string OperationId { get; set; }
        public string[] CustomerIds { get; set; }
        public string EmployeeId { get; set; }
        public string[] StoreIds { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
