using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class CouponEntity : Entity, ICloneable
    {
        [StringLength(64)]
        public string Code { get; set; }

        public string ShoppingCartId { get; set; }
        public virtual ShoppingCartEntity ShoppingCart { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as CouponEntity;

            if (ShoppingCart != null)
            {
                result.ShoppingCart = ShoppingCart.Clone() as ShoppingCartEntity;
            }

            return result;
        }

        #endregion
    }
}
