using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PaymentModule.Data.Model
{
    public class StorePaymentMethodEntity : Entity
    {
        [Required]
        [StringLength(128)]
        public string Code { get; set; }

        public int Priority { get; set; }

        [StringLength(2048)]
        public string LogoUrl { get; set; }

        public bool IsActive { get; set; }

        public bool IsAvailableForPartial { get; set; }

        public string TypeName { get; set; }

        public string StoreId { get; set; }

        public virtual PaymentMethod ToModel(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException(nameof(paymentMethod));

            paymentMethod.Id = Id;
            paymentMethod.IsActive = IsActive;
            paymentMethod.Code = Code;
            paymentMethod.IsAvailableForPartial = IsAvailableForPartial;
            paymentMethod.LogoUrl = LogoUrl;
            paymentMethod.Priority = Priority;
            paymentMethod.StoreId = StoreId;

            return paymentMethod;
        }

        public virtual StorePaymentMethodEntity FromModel(PaymentMethod paymentMethod, PrimaryKeyResolvingMap pkMap)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException(nameof(paymentMethod));

            pkMap.AddPair(paymentMethod, this);

            Id = paymentMethod.Id;
            IsActive = paymentMethod.IsActive;
            Code = paymentMethod.Code;
            IsAvailableForPartial = paymentMethod.IsAvailableForPartial;
            LogoUrl = paymentMethod.LogoUrl;
            Priority = paymentMethod.Priority;
            StoreId = paymentMethod.StoreId;
            TypeName = paymentMethod.TypeName;

            return this;
        }

        public virtual void Patch(StorePaymentMethodEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.IsActive = IsActive;
            target.Code = Code;
            target.IsAvailableForPartial = IsAvailableForPartial;
            target.LogoUrl = LogoUrl;
            target.Priority = Priority;
            target.StoreId = StoreId;
        }
    }
}
