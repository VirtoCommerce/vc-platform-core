using System.Collections.Generic;

namespace VirtoCommerce.CoreModule.Core.Common
{
    public interface IHasDiscounts
    {
        ICollection<Discount> Discounts { get; set; }
    }
}
