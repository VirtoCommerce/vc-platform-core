using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.StoreModule.Data.Model
{
	public class StoreShippingMethodEntity : Entity
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

		[StringLength(64)]
		public string TaxType { get; set; }

		public bool IsActive { get; set; }


		#region Navigation Properties

		public string StoreId { get; set; }

		public StoreEntity Store { get; set; }

        #endregion

        public virtual ShippingMethod ToModel(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException(nameof(shippingMethod));

            shippingMethod.IsActive = IsActive;
            shippingMethod.Code = Code;
            shippingMethod.Description = Description;
            shippingMethod.TaxType = TaxType;
            shippingMethod.LogoUrl = LogoUrl;
            shippingMethod.Name = Name;
            shippingMethod.Priority = Priority;

            return shippingMethod;
        }

        public virtual StoreShippingMethodEntity FromModel(ShippingMethod shippingMethod, PrimaryKeyResolvingMap pkMap)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException(nameof(shippingMethod));

            pkMap.AddPair(shippingMethod, this);

            IsActive = shippingMethod.IsActive;
            Code = shippingMethod.Code;
            Description = shippingMethod.Description;
            TaxType = shippingMethod.TaxType;
            LogoUrl = shippingMethod.LogoUrl;
            Name = shippingMethod.Name;
            Priority = shippingMethod.Priority;

            return this;
        }

        public virtual void Patch(StoreShippingMethodEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.IsActive = IsActive;
            target.Code = Code;
            target.Description = Description;
            target.TaxType = TaxType;
            target.LogoUrl = LogoUrl;
            target.Name = Name;
            target.Priority = Priority;
        }
    }
}
