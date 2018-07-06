using System;
using VirtoCommerce.CoreModule.Core.Model.Shipping;

namespace VirtoCommerce.CoreModule.Core.Registrars
{
	public interface IShippingMethodsRegistrar
	{
		ShippingMethod[] GetAllShippingMethods();
		void RegisterShippingMethod(Func<ShippingMethod> methodFactory);
	}
}
