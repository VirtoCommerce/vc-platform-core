using System.Collections.Generic;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class TaxDetailEntityComparer : IEqualityComparer<TaxDetailEntity>
    {
        public bool Equals(TaxDetailEntity x, TaxDetailEntity y)
        {
            bool equals;

            if (x != null && y != null)
            {
                equals = x.Name == y.Name;
            }
            else
            {
                equals = false;
            }

            return equals;
        }

        public int GetHashCode(TaxDetailEntity obj)
        {
            // Using prime numbers
            var hashCode = 17 * obj.Name?.GetHashCode() ?? 19;
            return hashCode;
        }
    }
}
