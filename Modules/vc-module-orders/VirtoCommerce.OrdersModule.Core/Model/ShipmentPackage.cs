using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Core.Model
{
    public class ShipmentPackage : AuditableEntity, IHasDimension, ICloneable
    {
        public string BarCode { get; set; }
        public string PackageType { get; set; }

        public ICollection<ShipmentItem> Items { get; set; }

        #region IHaveDimension Members
        public string WeightUnit { get; set; }
        public decimal? Weight { get; set; }

        public string MeasureUnit { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as ShipmentPackage;

            result.Items = Items?.Select(x => x.Clone()).OfType<ShipmentItem>().ToList();

            return result;
        }

        #endregion
    }
}
