using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    public class ShipmentPackageEntity : AuditableEntity
    {
        [StringLength(128)]
        public string BarCode { get; set; }
        [StringLength(64)]
        public string PackageType { get; set; }

        [StringLength(32)]
        public string WeightUnit { get; set; }
        public decimal? Weight { get; set; }
        [StringLength(32)]
        public string MeasureUnit { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }

        #region Navigation Properties

        public string ShipmentId { get; set; }
        public virtual ShipmentEntity Shipment { get; set; }

        public virtual ObservableCollection<ShipmentItemEntity> Items { get; set; } = new NullCollection<ShipmentItemEntity>();

        #endregion

        public virtual ShipmentPackage ToModel(ShipmentPackage package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            package.Id = Id;
            package.CreatedDate = CreatedDate;
            package.CreatedBy = CreatedBy;
            package.ModifiedDate = ModifiedDate;
            package.ModifiedBy = ModifiedBy;

            package.PackageType = PackageType;
            package.Weight = Weight;
            package.Height = Height;
            package.Width = Width;
            package.MeasureUnit = MeasureUnit;
            package.WeightUnit = WeightUnit;
            package.Length = Length;

            package.Items = Items.Select(x => x.ToModel(AbstractTypeFactory<ShipmentItem>.TryCreateInstance())).ToList();

            return package;
        }

        public virtual ShipmentPackageEntity FromModel(ShipmentPackage package, PrimaryKeyResolvingMap pkMap)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            Id = package.Id;
            CreatedDate = package.CreatedDate;
            CreatedBy = package.CreatedBy;
            ModifiedDate = package.ModifiedDate;
            ModifiedBy = package.ModifiedBy;

            PackageType = package.PackageType;
            Weight = package.Weight;
            Height = package.Height;
            Width = package.Width;
            MeasureUnit = package.MeasureUnit;
            WeightUnit = package.WeightUnit;
            Length = package.Length;

            pkMap.AddPair(package, this);

            if (!Items.IsNullOrEmpty())
            {
                Items = new ObservableCollection<ShipmentItemEntity>(package.Items.Select(x => AbstractTypeFactory<ShipmentItemEntity>.TryCreateInstance().FromModel(x, pkMap)));
                foreach (var shipmentItem in Items)
                {
                    shipmentItem.ShipmentPackageId = Id;
                }
            }

            return this;
        }

        public virtual void Patch(ShipmentPackageEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.PackageType = PackageType;
            target.ShipmentId = ShipmentId;
            target.Weight = Weight;
            target.Height = Height;
            target.Width = Width;
            target.MeasureUnit = MeasureUnit;
            target.WeightUnit = WeightUnit;
            target.Length = Length;

            if (!Items.IsNullCollection())
            {
                Items.Patch(target.Items, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
            }
        }
    }
}
