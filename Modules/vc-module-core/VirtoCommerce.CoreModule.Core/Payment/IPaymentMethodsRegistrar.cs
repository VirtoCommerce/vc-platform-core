using System;

namespace VirtoCommerce.CoreModule.Core.Payment
{
    public interface IPaymentMethodsRegistrar
    {
        PaymentMethod[] GetAllPaymentMethods();
        void RegisterPaymentMethod(Func<PaymentMethod> methodGetter);
    }
}
