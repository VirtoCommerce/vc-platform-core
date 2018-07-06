using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CoreModule.Data.Shipping
{
	public class FixedRateShippingMethod : ShippingMethod
	{
		public FixedRateShippingMethod()
			: base("FixedRate")
		{
		}

		public FixedRateShippingMethod(params SettingEntry[] settings)
			: base("FixedRate")
		{
			Settings = settings;
		}

        private decimal GroundOptionRate
        {
            get
            {
                return Settings.GetSettingValue<decimal>("VirtoCommerce.Core.FixedRateShippingMethod.Ground.Rate", 0);          
            }
        }

        private decimal AirOptionRate
		{
			get
			{
                return Settings.GetSettingValue<decimal>("VirtoCommerce.Core.FixedRateShippingMethod.Air.Rate", 0);
            }
		}

		public override IEnumerable<ShippingRate> CalculateRates(Domain.Common.IEvaluationContext context)
		{
			var shippingEvalContext = context as ShippingEvaluationContext;
			if(shippingEvalContext == null)
			{
				throw new NullReferenceException("shippingEvalContext");
			}

            return new ShippingRate[]
            {
                new ShippingRate { Rate = GroundOptionRate, Currency = shippingEvalContext.ShoppingCart.Currency, ShippingMethod = this, OptionName = "Ground", OptionDescription = "Ground shipping" },
                new ShippingRate { Rate = AirOptionRate, Currency = shippingEvalContext.ShoppingCart.Currency, ShippingMethod = this, OptionName = "Air", OptionDescription = "Air shipping" }
            };
        }
	}
}
