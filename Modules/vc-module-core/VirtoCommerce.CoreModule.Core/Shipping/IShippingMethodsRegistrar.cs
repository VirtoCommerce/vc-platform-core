using System;

namespace VirtoCommerce.CoreModule.Core.Shipping
{
    public interface IShippingMethodsRegistrar
    {
        ShippingMethod[] GetAllShippingMethods();
        void RegisterShippingMethod(Func<ShippingMethod> methodFactory);
    }
}
