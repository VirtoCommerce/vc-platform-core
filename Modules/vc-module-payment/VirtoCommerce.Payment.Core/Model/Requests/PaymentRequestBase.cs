using System.Collections.Specialized;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PaymentModule.Model.Requests
{
    public abstract class PaymentRequestBase : ValueObject
    {
        public string PaymentId { get; set; }
        public IEntity Payment { get; set; }

        public string OrderId { get; set; }
        public IEntity Order { get; set; }

        public string StoreId { get; set; }

        public string OuterId { get; set; }

        public NameValueCollection Parameters { get; set; }
    }
}
