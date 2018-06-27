using System;
using VirtoCommerce.CoreModule.Core.Model.Shipping;

namespace VirtoCommerce.CoreModule.Core.Services
{
	public interface IShippingMethodsService
	{
		ShippingMethod[] GetAllShippingMethods();
		void RegisterShippingMethod(Func<ShippingMethod> methodFactory);
	}
}
