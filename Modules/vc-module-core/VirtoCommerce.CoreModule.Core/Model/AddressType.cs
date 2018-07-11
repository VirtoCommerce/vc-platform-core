using System;

namespace VirtoCommerce.CoreModule.Core.Model
{
	[Flags]
	public enum AddressType
	{
		Billing = 1,
		Shipping = 2,
		BillingAndShipping = Billing | Shipping
	}
}
