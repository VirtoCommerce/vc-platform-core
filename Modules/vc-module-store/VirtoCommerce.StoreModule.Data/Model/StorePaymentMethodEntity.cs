using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.StoreModule.Data.Model
{
    public class StorePaymentMethodEntity : Entity
    {
        [Required]
        [StringLength(128)]
        public string Code { get; set; }

        public int Priority { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        public string Description { get; set; }

        [StringLength(2048)]
        public string LogoUrl { get; set; }

        public bool IsActive { get; set; }

        public bool IsAvailableForPartial { get; set; }

        #region Navigation Properties

        public string StoreId { get; set; }

        public StoreEntity Store { get; set; }

        #endregion

        public virtual PaymentMethod ToModel(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException(nameof(paymentMethod));

            paymentMethod.IsActive = IsActive;
            paymentMethod.Code = Code;
            paymentMethod.Description = Description;
            paymentMethod.IsAvailableForPartial = IsAvailableForPartial;
            paymentMethod.LogoUrl = LogoUrl;
            paymentMethod.Name = Name;
            paymentMethod.Priority = Priority;

            return paymentMethod;
        }

        public virtual StorePaymentMethodEntity FromModel(PaymentMethod paymentMethod, PrimaryKeyResolvingMap pkMap)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException(nameof(paymentMethod));

            pkMap.AddPair(paymentMethod, this);

            IsActive = paymentMethod.IsActive;
            Code = paymentMethod.Code;
            Description = paymentMethod.Description;
            IsAvailableForPartial = paymentMethod.IsAvailableForPartial;
            LogoUrl = paymentMethod.LogoUrl;
            Name = paymentMethod.Name;
            Priority = paymentMethod.Priority;

            return this;
        }

        public virtual void Patch(StorePaymentMethodEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.IsActive = IsActive;
            target.Code = Code;
            target.Description = Description;
            target.IsAvailableForPartial = IsAvailableForPartial;
            target.LogoUrl = LogoUrl;
            target.Name = Name;
            target.Priority = Priority;
        }
    }
}
