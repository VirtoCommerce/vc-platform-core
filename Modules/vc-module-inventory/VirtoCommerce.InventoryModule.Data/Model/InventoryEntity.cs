using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.InventoryModule.Data.Model
{
    public class InventoryEntity : AuditableEntity, IHasOuterId
    {
        [Required]
        public decimal InStockQuantity { get; set; }

        [Required]
        public decimal ReservedQuantity { get; set; }

        [Required]
        public decimal ReorderMinQuantity { get; set; }

        public decimal PreorderQuantity { get; set; }

        public decimal BackorderQuantity { get; set; }

        public bool AllowBackorder { get; set; }

        public bool AllowPreorder { get; set; }

        [Required]
        public int Status { get; set; }

        /// <summary>
        /// The date from when the preorder is allowed. 
        /// If not set AllowPreorder has no effect and not available
        /// </summary>
        public DateTime? PreorderAvailabilityDate { get; set; }

        /// <summary>
        /// The date from when the backorder is allowed. 
        /// If not set AllowBackorder has no effect and not available
        /// </summary>
        public DateTime? BackorderAvailabilityDate { get; set; }

        [Required]
        [StringLength(128)]
        public string Sku { get; set; }

        [StringLength(128)]
        public string OuterId { get; set; }

        #region Navigation properties

        public string FulfillmentCenterId { get; set; }
        public FulfillmentCenterEntity FulfillmentCenter { get; set; }

        #endregion

        public virtual InventoryInfo ToModel(InventoryInfo inventory)
        {
            inventory.Id = Id;
            inventory.CreatedBy = CreatedBy;
            inventory.CreatedDate = CreatedDate;
            inventory.ModifiedBy = ModifiedBy;
            inventory.ModifiedDate = ModifiedDate;
            inventory.OuterId = OuterId;

            inventory.AllowBackorder = AllowBackorder;
            inventory.AllowPreorder = AllowPreorder;
            inventory.BackorderAvailabilityDate = BackorderAvailabilityDate;
            inventory.BackorderQuantity = (long)BackorderQuantity;
            inventory.FulfillmentCenterId = FulfillmentCenterId;
            inventory.InStockQuantity = (long)InStockQuantity;
            inventory.PreorderAvailabilityDate = PreorderAvailabilityDate;
            inventory.PreorderQuantity = (long)PreorderQuantity;
            inventory.ReorderMinQuantity = (long)ReorderMinQuantity;
            inventory.ProductId = Sku;
            inventory.ReservedQuantity = (long)ReservedQuantity;
            inventory.Status = EnumUtility.SafeParse(Status.ToString(), InventoryStatus.Enabled);

            if (FulfillmentCenter != null)
            {
                inventory.FulfillmentCenter = FulfillmentCenter.ToModel(AbstractTypeFactory<FulfillmentCenter>.TryCreateInstance());
            }

            return inventory;
        }

        public virtual InventoryEntity FromModel(InventoryInfo inventory)
        {
            Id = inventory.Id;
            CreatedBy = inventory.CreatedBy;
            CreatedDate = inventory.CreatedDate;
            ModifiedBy = inventory.ModifiedBy;
            ModifiedDate = inventory.ModifiedDate;
            OuterId = inventory.OuterId;

            AllowBackorder = inventory.AllowBackorder;
            AllowPreorder = inventory.AllowPreorder;
            BackorderAvailabilityDate = inventory.BackorderAvailabilityDate;
            BackorderQuantity = inventory.BackorderQuantity;
            FulfillmentCenterId = inventory.FulfillmentCenterId;
            InStockQuantity = inventory.InStockQuantity;
            PreorderAvailabilityDate = inventory.PreorderAvailabilityDate;
            PreorderQuantity = inventory.PreorderQuantity;
            ReorderMinQuantity = inventory.ReorderMinQuantity;
            Sku = inventory.ProductId;
            ReservedQuantity = inventory.ReservedQuantity;
            Status = (int)inventory.Status;

            return this;
        }

        public virtual void Patch(InventoryEntity target)
        {
            target.AllowBackorder = AllowBackorder;
            target.AllowPreorder = AllowPreorder;
            target.BackorderAvailabilityDate = BackorderAvailabilityDate;
            target.BackorderQuantity = BackorderQuantity;
            target.FulfillmentCenterId = FulfillmentCenterId;
            target.InStockQuantity = InStockQuantity;
            target.PreorderAvailabilityDate = PreorderAvailabilityDate;
            target.PreorderQuantity = PreorderQuantity;
            target.ReorderMinQuantity = ReorderMinQuantity;
            target.Sku = Sku;
            target.ReservedQuantity = ReservedQuantity;
            target.Status = Status;
        }
    }
}
