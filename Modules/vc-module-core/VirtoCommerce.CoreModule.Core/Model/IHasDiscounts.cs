using System.Collections.Generic;

namespace VirtoCommerce.CoreModule.Core.Model
{
    public interface IHasDiscounts
    {
        ICollection<Discount> Discounts { get; set; }
    }
}
