using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.PaymentModule.Core.Models;
using VirtoCommerce.PaymentModule.Core.Services;

namespace VirtoCommerce.PaymentModule.Data
{
    public class PaymentMethodsRegistrar : IPaymentMethodsRegistrar
    {
        private readonly List<Func<PaymentMethod>> _paymentMethods = new List<Func<PaymentMethod>>();

        #region IPaymentMethodsRegistrar Members

        public PaymentMethod[] GetAllPaymentMethods()
        {
            return _paymentMethods.Select(x => x()).ToArray();
        }

        public void RegisterPaymentMethod(Func<PaymentMethod> methodGetter)
        {
            if (methodGetter == null)
            {
                throw new ArgumentNullException(nameof(methodGetter));
            }

            _paymentMethods.Add(methodGetter);
        }

        #endregion
    }
}
