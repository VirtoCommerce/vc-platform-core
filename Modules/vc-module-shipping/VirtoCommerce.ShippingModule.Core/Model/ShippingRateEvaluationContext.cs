using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.ShippingModule.Core.Model
{
    public class ShippingRateEvaluationContext : IEvaluationContext
    {
        public string Currency { get; set; }
    }
}
