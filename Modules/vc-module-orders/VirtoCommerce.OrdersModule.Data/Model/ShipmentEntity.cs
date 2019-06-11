using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using Address = VirtoCommerce.OrdersModule.Core.Model.Address;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    public class ShipmentEntity : OperationEntity, ISupportPartialPriceUpdate
    {
        [StringLength(64)]
        public string OrganizationId { get; set; }
        [StringLength(255)]
        public string OrganizationName { get; set; }

        [StringLength(64)]
        public string FulfillmentCenterId { get; set; }
        [StringLength(255)]
        public string FulfillmentCenterName { get; set; }

        [StringLength(64)]
        public string EmployeeId { get; set; }
        [StringLength(255)]
        public string EmployeeName { get; set; }

        [StringLength(64)]
        public string ShipmentMethodCode { get; set; }
        [StringLength(64)]
        public string ShipmentMethodOption { get; set; }

        public decimal? VolumetricWeight { get; set; }
        [StringLength(32)]
        public string WeightUnit { get; set; }
        public decimal? Weight { get; set; }
        [StringLength(32)]
        public string MeasureUnit { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }

        [StringLength(64)]
        public string TaxType { get; set; }

        [Column(TypeName = "Money")]
        public decimal? Price { get; set; }
        [Column(TypeName = "Money")]
        public decimal? PriceWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal? DiscountAmount { get; set; }
        [Column(TypeName = "Money")]
        public decimal? DiscountAmountWithTax { get; set; }


        [Column(TypeName = "Money")]
        public decimal? Total { get; set; }
        [Column(TypeName = "Money")]
        public decimal? TotalWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal? TaxTotal { get; set; }
        public decimal? TaxPercentRate { get; set; }

        public string CustomerOrderId { get; set; }
        public virtual CustomerOrderEntity CustomerOrder { get; set; }

        public virtual ObservableCollection<ShipmentItemEntity> Items { get; set; } = new NullCollection<ShipmentItemEntity>();
        public virtual ObservableCollection<ShipmentPackageEntity> Packages { get; set; } = new NullCollection<ShipmentPackageEntity>();
        public virtual ObservableCollection<PaymentInEntity> InPayments { get; set; } = new NullCollection<PaymentInEntity>();
        public virtual ObservableCollection<AddressEntity> Addresses { get; set; } = new NullCollection<AddressEntity>();
        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; } = new NullCollection<DiscountEntity>();
        public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; } = new NullCollection<TaxDetailEntity>();


        public override OrderOperation ToModel(OrderOperation operation)
        {
            var shipment = operation as Shipment;
            if (shipment == null)
            {
                throw new ArgumentException(@"operation argument must be of type Shipment", nameof(operation));
            }

            if (!Addresses.IsNullOrEmpty())
            {
                shipment.DeliveryAddress = Addresses.First().ToModel(AbstractTypeFactory<Address>.TryCreateInstance());
            }

            shipment.Id = Id;
            shipment.CreatedDate = CreatedDate;
            shipment.CreatedBy = CreatedBy;
            shipment.ModifiedDate = ModifiedDate;
            shipment.ModifiedBy = ModifiedBy;

            shipment.Price = Price;
            shipment.PriceWithTax = PriceWithTax;
            shipment.DiscountAmount = DiscountAmount;
            shipment.DiscountAmountWithTax = DiscountAmountWithTax;
            shipment.FulfillmentCenterId = FulfillmentCenterId;
            shipment.FulfillmentCenterName = FulfillmentCenterName;
            shipment.OrganizationId = OrganizationId;
            shipment.OrganizationName = OrganizationName;
            shipment.EmployeeId = EmployeeId;
            shipment.EmployeeName = EmployeeName;
            shipment.ShipmentMethodCode = ShipmentMethodCode;
            shipment.ShipmentMethodOption = ShipmentMethodOption;
            shipment.Height = Height;
            shipment.Length = Length;
            shipment.Weight = Weight;
            shipment.Height = Height;
            shipment.Width = Width;
            shipment.MeasureUnit = MeasureUnit;
            shipment.WeightUnit = WeightUnit;
            shipment.Length = Length;
            shipment.TaxType = TaxType;
            shipment.TaxPercentRate = TaxPercentRate;
            shipment.TaxTotal = TaxTotal;
            shipment.Total = Total;
            shipment.TotalWithTax = TotalWithTax;

            shipment.Discounts = Discounts.Select(x => x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            shipment.Items = Items.Select(x => x.ToModel(AbstractTypeFactory<ShipmentItem>.TryCreateInstance())).ToList();
            shipment.InPayments = InPayments.Select(x => x.ToModel(AbstractTypeFactory<PaymentIn>.TryCreateInstance())).OfType<PaymentIn>().ToList();
            shipment.Packages = Packages.Select(x => x.ToModel(AbstractTypeFactory<ShipmentPackage>.TryCreateInstance())).ToList();
            shipment.TaxDetails = TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();

            base.ToModel(shipment);

            operation.Sum = shipment.TotalWithTax;

            return shipment;
        }

        public override OperationEntity FromModel(OrderOperation operation, PrimaryKeyResolvingMap pkMap)
        {
            var shipment = operation as Shipment;
            if (shipment == null)
            {
                throw new ArgumentException(@"operation argument must be of type Shipment", nameof(operation));
            }

            base.FromModel(shipment, pkMap);

            Id = shipment.Id;
            CreatedDate = shipment.CreatedDate;
            CreatedBy = shipment.CreatedBy;
            ModifiedDate = shipment.ModifiedDate;
            ModifiedBy = shipment.ModifiedBy;

            Price = shipment.Price;
            PriceWithTax = shipment.PriceWithTax;
            DiscountAmount = shipment.DiscountAmount;
            DiscountAmountWithTax = shipment.DiscountAmountWithTax;
            FulfillmentCenterId = shipment.FulfillmentCenterId;
            FulfillmentCenterName = shipment.FulfillmentCenterName;
            OrganizationId = shipment.OrganizationId;
            OrganizationName = shipment.OrganizationName;
            EmployeeId = shipment.EmployeeId;
            EmployeeName = shipment.EmployeeName;
            ShipmentMethodCode = shipment.ShipmentMethodCode;
            ShipmentMethodOption = shipment.ShipmentMethodOption;
            Height = shipment.Height;
            Length = shipment.Length;
            Weight = shipment.Weight;
            Height = shipment.Height;
            Width = shipment.Width;
            MeasureUnit = shipment.MeasureUnit;
            WeightUnit = shipment.WeightUnit;
            Length = shipment.Length;
            TaxType = shipment.TaxType;
            TaxPercentRate = shipment.TaxPercentRate;
            TaxTotal = shipment.TaxTotal;
            Total = shipment.Total;
            TotalWithTax = shipment.TotalWithTax;

            //Allow to empty address
            Addresses = new ObservableCollection<AddressEntity>();

            if (shipment.ShippingMethod != null)
            {
                ShipmentMethodCode = shipment.ShippingMethod.Code;
                ShipmentMethodOption = shipment.ShipmentMethodOption;
            }

            if (shipment.DeliveryAddress != null)
            {
                Addresses = new ObservableCollection<AddressEntity>(new[] { AbstractTypeFactory<AddressEntity>.TryCreateInstance().FromModel(shipment.DeliveryAddress) });
            }

            if (shipment.Items != null)
            {
                Items = new ObservableCollection<ShipmentItemEntity>(shipment.Items.Select(x => AbstractTypeFactory<ShipmentItemEntity>.TryCreateInstance().FromModel(x, pkMap)));
                foreach (var shipmentItem in Items)
                {
                    shipmentItem.ShipmentId = Id;
                }
            }

            if (shipment.Packages != null)
            {
                Packages = new ObservableCollection<ShipmentPackageEntity>(shipment.Packages.Select(x => AbstractTypeFactory<ShipmentPackageEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }

            if (shipment.TaxDetails != null)
            {
                TaxDetails = new ObservableCollection<TaxDetailEntity>(shipment.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }

            if (shipment.Discounts != null)
            {
                Discounts = new ObservableCollection<DiscountEntity>(shipment.Discounts.Select(x => AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }

            Sum = shipment.TotalWithTax ?? Sum;

            return this;
        }

        public override void Patch(OperationEntity operation)
        {
            var target = operation as ShipmentEntity;
            if (target == null)
            {
                throw new ArgumentException(@"operation argument must be of type ShipmentEntity", nameof(operation));
            }

            base.Patch(operation);

            target.FulfillmentCenterId = FulfillmentCenterId;
            target.FulfillmentCenterName = FulfillmentCenterName;
            target.OrganizationId = OrganizationId;
            target.OrganizationName = OrganizationName;
            target.EmployeeId = EmployeeId;
            target.EmployeeName = EmployeeName;
            target.ShipmentMethodCode = ShipmentMethodCode;
            target.ShipmentMethodOption = ShipmentMethodOption;
            target.Height = Height;
            target.Length = Length;
            target.Weight = Weight;
            target.Height = Height;
            target.Width = Width;
            target.MeasureUnit = MeasureUnit;
            target.WeightUnit = WeightUnit;
            target.Length = Length;
            target.TaxType = TaxType;

            target.Price = Price;
            target.PriceWithTax = PriceWithTax;
            target.DiscountAmount = DiscountAmount;
            target.DiscountAmountWithTax = DiscountAmountWithTax;
            target.TaxPercentRate = TaxPercentRate;
            target.TaxTotal = TaxTotal;
            target.Total = Total;
            target.TotalWithTax = TotalWithTax;

            if (!InPayments.IsNullCollection())
            {
                InPayments.Patch(target.InPayments, (sourcePayment, targetPayment) => sourcePayment.Patch(targetPayment));
            }

            if (!Items.IsNullCollection())
            {
                Items.Patch(target.Items, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
            }

            if (!Discounts.IsNullCollection())
            {
                var discountComparer = AnonymousComparer.Create((DiscountEntity x) => x.PromotionId);
                Discounts.Patch(target.Discounts, discountComparer, (sourceDiscount, targetDiscount) => sourceDiscount.Patch(targetDiscount));
            }

            if (!Addresses.IsNullCollection())
            {
                Addresses.Patch(target.Addresses, (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
            }

            if (!Packages.IsNullCollection())
            {
                Packages.Patch(target.Packages, (sourcePackage, targetPackage) => sourcePackage.Patch(targetPackage));
            }

            if (!TaxDetails.IsNullCollection())
            {
                var taxDetailComparer = AnonymousComparer.Create((TaxDetailEntity x) => x.Name);
                TaxDetails.Patch(target.TaxDetails, taxDetailComparer, (sourceTaxDetail, targetTaxDetail) => sourceTaxDetail.Patch(targetTaxDetail));
            }
        }
        public virtual void ResetPrices()
        {
            Price = null;
            PriceWithTax = null;
            DiscountAmount = null;
            DiscountAmountWithTax = null;
            Total = null;
            TotalWithTax = null;
            TaxTotal = null;
            TaxPercentRate = null;
            Sum = null;
        }
    }
}
