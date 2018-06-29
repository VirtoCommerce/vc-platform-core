using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Model.Shipping;

namespace VirtoCommerce.CoreModule.Core.Services
{
	public class ShippingMethodsServiceImpl : IShippingMethodsService
	{
		private List<Func<ShippingMethod>> _shippingMethods = new List<Func<ShippingMethod>>();
		
		#region IShippingService Members

		public ShippingMethod[] GetAllShippingMethods()
		{
			return _shippingMethods.Select(x => x()).ToArray();
		}

		public void RegisterShippingMethod(Func<ShippingMethod> methodFactory)
		{
			if (methodFactory == null)
			{
				throw new ArgumentNullException("methodFactory");
			}

			_shippingMethods.Add(methodFactory);
		}

		#endregion
	}
}
