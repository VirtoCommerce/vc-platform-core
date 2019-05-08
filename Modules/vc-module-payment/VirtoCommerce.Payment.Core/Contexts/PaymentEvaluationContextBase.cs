using System.Collections.Specialized;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PaymentModule.Core.Contexts
{
    public abstract class PaymentEvaluationContextBase : ValueObject, IEvaluationContext
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public NameValueCollection Parameters { get; set; }
    }
}
