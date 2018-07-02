using System.Collections.Generic;
using VirtoCommerce.Domain.Commerce.Model;

namespace VirtoCommerce.CoreModule.Core.Commerce.Model
{
    public interface IHasDiscounts
    {
        ICollection<Discount> Discounts { get; set; }
    }
}
