using System.Collections.Generic;

namespace VirtoCommerce.CoreModule.Core.Tax
{
    public interface IHasTaxDetalization
    {
        ICollection<TaxDetail> TaxDetails { get; set; }
    }
}
