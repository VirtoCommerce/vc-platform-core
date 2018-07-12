using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Shipping;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CoreModule.Data.Shipping
{
    public class FixedRateShippingMethod : ShippingMethod
    {
        private decimal GroundOptionRate
        {
            get
            {
                return Settings.GetSettingValue(ModuleConstants.Settings.General.FixedRateShippingMethodGroundRate.Name, 0m);
            }
        }

        private decimal AirOptionRate
        {
            get
            {
                return Settings.GetSettingValue(ModuleConstants.Settings.General.FixedRateShippingMethodAirRate.Name, 0m);
            }
        }

        public override IEnumerable<ShippingRate> CalculateRates(IEvaluationContext context)
        {
            if (!(context is ShippingRateEvaluationContext shippingContext))
            {
                throw new ArgumentException(nameof(context));
            }
            return new ShippingRate[]
            {
                new ShippingRate { Rate = GroundOptionRate, Currency = shippingContext.Currency, ShippingMethod = this, OptionName = "Ground" },
                new ShippingRate { Rate = AirOptionRate, Currency = shippingContext.Currency, ShippingMethod = this, OptionName = "Air" }
            };
        }
    }
}
