using System.Collections.Specialized;
using VirtoCommerce.CoreModule.Common;

namespace VirtoCommerce.CoreModule.Core.Model.Payment
{
    public abstract class PaymentEvaluationContextBase : IEvaluationContext
    {
        //TODO
        //public PaymentIn Payment { get; set; }
        //public CustomerOrder Order { get; set; }
        public NameValueCollection Parameters { get; set; }
    }
}
