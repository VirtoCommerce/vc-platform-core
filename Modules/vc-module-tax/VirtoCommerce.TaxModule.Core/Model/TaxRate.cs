using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.TaxModule.Core.Model
{
    public class TaxRate : ValueObject
    {
        public decimal Rate { get; set; }
        public string Currency { get; set; }

        public TaxLine Line { get; set; }
        public TaxProvider TaxProvider { get; set; }
    }
}
