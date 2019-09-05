using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    public class TaxDetailEntity : Entity
    {
        public decimal Rate { get; set; }

        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        [StringLength(1024)]
        public string Name { get; set; }

        #region Navigation Properties

        public string CustomerOrderId { get; set; }
        public virtual CustomerOrderEntity CustomerOrder { get; set; }

        public string ShipmentId { get; set; }
        public virtual ShipmentEntity Shipment { get; set; }

        public string LineItemId { get; set; }
        public virtual LineItemEntity LineItem { get; set; }

        public string PaymentInId { get; set; }
        public virtual PaymentInEntity PaymentIn { get; set; }

        #endregion

        public virtual TaxDetail ToModel(TaxDetail taxDetail)
        {
            if (taxDetail == null)
                throw new ArgumentNullException(nameof(taxDetail));

            taxDetail.Name = Name;
            taxDetail.Rate = Rate;
            taxDetail.Amount = Amount;

            return taxDetail;
        }

        public virtual TaxDetailEntity FromModel(TaxDetail taxDetail)
        {
            if (taxDetail == null)
                throw new ArgumentNullException(nameof(taxDetail));

            Name = taxDetail.Name;
            Rate = taxDetail.Rate;
            Amount = taxDetail.Amount;

            return this;
        }

        public virtual void Patch(TaxDetailEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.Rate = Rate;
            target.Amount = Amount;
        }
    }
}
