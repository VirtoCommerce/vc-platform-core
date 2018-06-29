using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Model.Payment;

namespace VirtoCommerce.CoreModule.Core.Services
{
	public class PaymentMethodsServiceImpl : IPaymentMethodsService
	{
		private List<Func<PaymentMethod>> _paymentMethods = new List<Func<PaymentMethod>>();

		#region IPaymentService Members

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
