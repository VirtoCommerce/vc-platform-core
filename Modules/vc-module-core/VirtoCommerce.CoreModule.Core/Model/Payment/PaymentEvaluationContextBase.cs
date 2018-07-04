using System.Collections.Specialized;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Model.Payment
{
    public abstract class PaymentEvaluationContextBase : ValueObject, IEvaluationContext
    {
        //TODO
        //public PaymentIn Payment { get; set; }
        //public CustomerOrder Order { get; set; }
        public NameValueCollection Parameters { get; set; }
    }
}
