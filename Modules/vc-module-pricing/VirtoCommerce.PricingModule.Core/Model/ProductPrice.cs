using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PricingModule.Core.Model
{
    public class ProductPrice : ValueObject, ICloneable
    {
        public string ProductId { get; set; }

        public CatalogProduct Product { get; set; }

        /// <summary>
        /// List prices for the products. It includes tiered prices also. (Depending on the quantity, for example)
        /// </summary>
        public ICollection<Price> Prices { get; set; }


        #region ICloneable members
        public override object Clone()
        {
            var result = base.Clone() as ProductPrice;
            if (Product != null)
            {
                result.Product = result.Product.Clone() as CatalogProduct;
            }
            if (Prices != null)
            {
                result.Prices = new List<Price>(Prices.Select(x => x.Clone() as Price));
            }
            return result;
        }
        #endregion

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ProductId;
        }
    }
}
