namespace VirtoCommerce.CoreModule.Core.Model.Payment
{
	public class VoidProcessPaymentResult : IProcessResult
	{
		public bool IsSuccess { get; set; }

		public string ErrorMessage { get; set; }

		public PaymentStatus NewPaymentStatus { get; set; }
	}
}
