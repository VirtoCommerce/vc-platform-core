using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Model.Payment;
using VirtoCommerce.CoreModule.Core.Registrars;

namespace VirtoCommerce.CoreModule.Data.Registrars
{
	public class PaymentMethodsRegistrarImpl : IPaymentMethodsRegistrar
	{
		private List<Func<PaymentMethod>> _paymentMethods = new List<Func<PaymentMethod>>();

        #region IPaymentMethodsRegistrar Members

        public PaymentMethod[] GetAllPaymentMethods()
		{
			return _paymentMethods.Select(x => x()).ToArray();
		}

		public void RegisterPaymentMethod(Func<PaymentMethod> methodGetter)
		{
			if (methodGetter == null)
			{
				throw new ArgumentNullException("methodGetter");
			}

			_paymentMethods.Add(methodGetter);
		}

		#endregion
	}
}
