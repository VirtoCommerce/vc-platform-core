using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Data
{
    public class FixedRateShippingMethod : ShippingMethod
    {
        public FixedRateShippingMethod() : base("FixedRateShippingMethod")
        {
        }

        private decimal GroundOptionRate
            => Settings.GetSettingValue(ModuleConstants.Settings.General.FixedRateShippingMethodGroundRate.Name, 0m);

        private decimal AirOptionRate
            => Settings.GetSettingValue(ModuleConstants.Settings.General.FixedRateShippingMethodAirRate.Name, 0m);

        public override IEnumerable<ShippingRate> CalculateRates(IEvaluationContext context)
        {
            if (!(context is ShippingRateEvaluationContext shippingContext))
            {
                throw new ArgumentException(nameof(context));
            }
            return new[]
            {
                new ShippingRate { Rate = GroundOptionRate, Currency = shippingContext.Currency, ShippingMethod = this, OptionName = "Ground" },
                new ShippingRate { Rate = AirOptionRate, Currency = shippingContext.Currency, ShippingMethod = this, OptionName = "Air" }
            };
        }
    }
}
