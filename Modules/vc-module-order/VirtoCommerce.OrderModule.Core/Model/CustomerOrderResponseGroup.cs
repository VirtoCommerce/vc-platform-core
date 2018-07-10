using System;

namespace VirtoCommerce.OrderModule.Core.Model
{
	[Flags]
	public enum CustomerOrderResponseGroup
	{
		Default = 0,
		WithItems = 1,
		WithInPayments = 2,
		WithShipments = 4,
		WithAddresses = 8,
		WithDiscounts = 16,
		Full = WithItems | WithInPayments | WithShipments | WithAddresses | WithDiscounts
	}
}
