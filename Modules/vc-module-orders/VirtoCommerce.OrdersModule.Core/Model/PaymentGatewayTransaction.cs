using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Core.Model
{
    //Container for the interactions associated with the payment gateway, which includes details for each action performed for the payment.
    public class PaymentGatewayTransaction : AuditableEntity, ICloneable
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Flag represent that current transaction is processed
        /// </summary>
        public bool IsProcessed { get; set; }
        /// <summary>
        /// Date when this transaction was handled 
        /// </summary>
        public DateTime? ProcessedDate { get; set; }
        public string ProcessError { get; set; }
        public int ProcessAttemptCount { get; set; }

        /// <summary>
        /// Raw request data
        /// </summary>
        public string RequestData { get; set; }     
        /// <summary>
        /// Raw response data
        /// </summary>
        public string ResponseData { get; set; }
        /// <summary>
        /// Gateway or VC response status code  
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gateway IP address
        /// </summary>
        public string GatewayIpAddress { get; set; }

        /// <summary>
        /// The type of payment interaction.The payment can be Capture or CheckReceived. 
        /// The value also includes customer payment interactions such as Website, Call, Store, or Unknown.
        /// </summary>
        public string Type { get; set;  }
        /// <summary>
        /// "Active", "Expired", and "Inactive" or other
        /// </summary>
        public string Status { get; set; }

        public string Note { get; set; }



        #region ICloneable members

        public virtual object Clone()
        {
            return MemberwiseClone() as PaymentGatewayTransaction;
        }

        #endregion
    }
}
