using System;
using VirtoCommerce.CoreModule.Core.Model.Payment;

namespace VirtoCommerce.CoreModule.Core.Registrars
{
	public interface IPaymentMethodsRegistrar
	{
		PaymentMethod[] GetAllPaymentMethods();
		void RegisterPaymentMethod(Func<PaymentMethod> methodGetter);
	}
}
