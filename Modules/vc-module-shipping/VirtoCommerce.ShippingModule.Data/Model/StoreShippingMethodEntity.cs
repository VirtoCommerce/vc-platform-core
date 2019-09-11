using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Data.Model
{
    public class StoreShippingMethodEntity : Entity
    {
        [Required]
        [StringLength(128)]
        public string Code { get; set; }

        public int Priority { get; set; }

        [StringLength(2048)]
        public string LogoUrl { get; set; }

        [StringLength(64)]
        public string TaxType { get; set; }

        public bool IsActive { get; set; }

        public string TypeName { get; set; }

        public string StoreId { get; set; }

        public virtual ShippingMethod ToModel(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException(nameof(shippingMethod));

            shippingMethod.Id = Id;
            shippingMethod.IsActive = IsActive;
            shippingMethod.Code = Code;
            shippingMethod.TaxType = TaxType;
            shippingMethod.LogoUrl = LogoUrl;
            shippingMethod.Priority = Priority;
            shippingMethod.StoreId = StoreId;

            return shippingMethod;
        }

        public virtual StoreShippingMethodEntity FromModel(ShippingMethod shippingMethod, PrimaryKeyResolvingMap pkMap)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException(nameof(shippingMethod));

            pkMap.AddPair(shippingMethod, this);

            Id = shippingMethod.Id;
            IsActive = shippingMethod.IsActive;
            Code = shippingMethod.Code;
            TaxType = shippingMethod.TaxType;
            LogoUrl = shippingMethod.LogoUrl;
            Priority = shippingMethod.Priority;
            StoreId = shippingMethod.StoreId;
            TypeName = shippingMethod.TypeName;

            return this;
        }

        public virtual void Patch(StoreShippingMethodEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.IsActive = IsActive;
            target.Code = Code;
            target.TaxType = TaxType;
            target.LogoUrl = LogoUrl;
            target.Priority = Priority;
            target.StoreId = StoreId;
        }
    }
}
