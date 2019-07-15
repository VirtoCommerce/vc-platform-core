using System.Collections.Generic;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    public interface ISupportPartialPriceUpdate
    {
        void ResetPrices();
        IEnumerable<decimal> GetNonCalculatablePrices();
    }
}
