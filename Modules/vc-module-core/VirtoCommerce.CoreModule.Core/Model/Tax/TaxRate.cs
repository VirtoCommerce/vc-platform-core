using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Model.Tax
{
    public class TaxRate : ValueObject
    {
        public decimal Rate { get; set; }
        public string Currency { get; set; }

        public TaxLine Line { get; set; }
        public TaxProvider TaxProvider { get; set; }
    }
}
