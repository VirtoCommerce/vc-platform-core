using System;
using VirtoCommerce.PaymentModule.Core.Models;

namespace VirtoCommerce.PaymentModule.Core.Services
{
    public interface IPaymentMethodsRegistrar
    {
        PaymentMethod[] GetAllPaymentMethods();

        void RegisterPaymentMethod(Func<PaymentMethod> methodGetter);
    }
}
