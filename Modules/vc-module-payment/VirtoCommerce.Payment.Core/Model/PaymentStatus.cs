namespace VirtoCommerce.PaymentModule.Core.Model
{
    /// <summary>
    /// Represents a payment status enumeration
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// New
        /// </summary>
        New,
        /// <summary>
        /// Pending
        /// </summary>
        Pending,
        /// <summary>
        /// Authorized
        /// </summary>
        Authorized,
        /// <summary>
        /// Paid
        /// </summary>
        Paid,
        /// <summary>
        /// Partially Refunded
        /// </summary>
        PartiallyRefunded,
        /// <summary>
        /// Refunded
        /// </summary>
        Refunded,
        /// <summary>
        /// Voided
        /// </summary>
        Voided,
        /// <summary>
        /// Custom
        /// </summary>
        Custom,
        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled
    }
}
