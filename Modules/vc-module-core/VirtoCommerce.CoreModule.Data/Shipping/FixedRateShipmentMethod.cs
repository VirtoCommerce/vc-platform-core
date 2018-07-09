using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Model.Shipping;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CoreModule.Data.Shipping
{
    //TODO need module cart
	//public class FixedRateShippingMethod : ShippingMethod
	//{
	//	public FixedRateShippingMethod()
	//	{
	//	}

	//	public FixedRateShippingMethod(params SettingEntry[] settings)
	//	{
	//		Settings = settings;
	//	}

 //       private decimal GroundOptionRate
 //       {
 //           get
 //           {
 //               return Settings.GetSettingValue<decimal>("VirtoCommerce.Core.FixedRateShippingMethod.Ground.Rate", 0);          
 //           }
 //       }

 //       private decimal AirOptionRate
	//	{
	//		get
	//		{
 //               return Settings.GetSettingValue<decimal>("VirtoCommerce.Core.FixedRateShippingMethod.Air.Rate", 0);
 //           }
	//	}

	//	public override IEnumerable<ShippingRate> CalculateRates(IEvaluationContext context)
	//	{
	//		var shippingEvalContext = context as ShippingEvaluationContext;
	//		if(shippingEvalContext == null)
	//		{
	//			throw new NullReferenceException("shippingEvalContext");
	//		}

 //           return new ShippingRate[]
 //           {
 //               new ShippingRate { Rate = GroundOptionRate, Currency = shippingEvalContext.ShoppingCart.Currency, ShippingMethod = this, OptionName = "Ground", OptionDescription = "Ground shipping" },
 //               new ShippingRate { Rate = AirOptionRate, Currency = shippingEvalContext.ShoppingCart.Currency, ShippingMethod = this, OptionName = "Air", OptionDescription = "Air shipping" }
 //           };
 //       }
	//}
}
