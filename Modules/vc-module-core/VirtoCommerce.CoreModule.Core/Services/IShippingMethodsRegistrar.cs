using System;
using VirtoCommerce.CoreModule.Core.Model.Shipping;

namespace VirtoCommerce.CoreModule.Core.Services
{
	public interface IShippingMethodsRegistrar
	{
		ShippingMethod[] GetAllShippingMethods();
		void RegisterShippingMethod(Func<ShippingMethod> methodFactory);
	}
}
