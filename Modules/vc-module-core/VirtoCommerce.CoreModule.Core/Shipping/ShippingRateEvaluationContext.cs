using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Shipping
{
    public class ShippingRateEvaluationContext : IEvaluationContext
    {
        public string Currency { get; set; }
    }
}
