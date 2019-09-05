using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.StoreModule.Data.Model
{
    public class StoreFulfillmentCenterEntity : Entity
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [StringLength(32)]
        public string Type { get; set; }

        [Required]
        [StringLength(128)]
        public string FulfillmentCenterId { get; set; }

        #region Navigation Properties

        public string StoreId { get; set; }
        public StoreEntity Store { get; set; }

        #endregion

        public virtual void Patch(StoreFulfillmentCenterEntity target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.Name = Name;
        }
    }
}
