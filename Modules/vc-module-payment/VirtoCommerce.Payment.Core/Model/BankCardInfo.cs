using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PaymentModule.Core.Model
{
    public class BankCardInfo : ValueObject
    {
        public string BankCardNumber { get; set; }
        public string BankCardType { get; set; }
        public int BankCardMonth { get; set; }
        public int BankCardYear { get; set; }
        public string BankCardCVV2 { get; set; }
        public string CardholderName { get; set; }
    }
}
