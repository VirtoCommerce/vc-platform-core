using System.Collections.Generic;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class DiscountEntityComparer : IEqualityComparer<DiscountEntity>
    {
        public bool Equals(DiscountEntity x, DiscountEntity y)
        {
            bool equals;

            if (x != null && y != null)
            {
                equals = x.PromotionId == y.PromotionId &&
                         x.PromotionDescription == y.PromotionDescription &&
                         x.CouponCode == y.CouponCode &&
                         x.Currency == y.Currency;
            }
            else
            {
                equals = false;
            }

            return equals;
        }

        public int GetHashCode(DiscountEntity obj)
        {
            var hashCode = 0;

            // Using prime numbers
            hashCode += 17 * obj.PromotionId?.GetHashCode() ?? 19;
            hashCode += 23 * obj.PromotionDescription?.GetHashCode() ?? 29;
            hashCode += 31 * obj.CouponCode?.GetHashCode() ?? 37;
            hashCode += 41 * obj.Currency?.GetHashCode() ?? 43;

            return hashCode;
        }
    }
}
