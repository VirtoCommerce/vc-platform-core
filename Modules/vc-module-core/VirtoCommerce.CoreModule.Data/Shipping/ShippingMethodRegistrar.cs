using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Shipping;

namespace VirtoCommerce.CoreModule.Data.Shipping
{
    public class ShippingMethodRegistrar : IShippingMethodsRegistrar
    {
        private List<Func<ShippingMethod>> _shippingMethods = new List<Func<ShippingMethod>>();

        #region IShippingMethodsRegistrar Members

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
