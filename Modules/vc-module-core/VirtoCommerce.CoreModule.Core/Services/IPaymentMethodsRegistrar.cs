using System;
using VirtoCommerce.CoreModule.Core.Model.Payment;

namespace VirtoCommerce.CoreModule.Core.Services
{
	public interface IPaymentMethodsRegistrar
	{
		PaymentMethod[] GetAllPaymentMethods();
		void RegisterPaymentMethod(Func<PaymentMethod> methodGetter);
	}
}
