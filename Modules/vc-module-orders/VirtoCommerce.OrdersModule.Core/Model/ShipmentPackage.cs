using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Core.Model
{
	public class ShipmentPackage : AuditableEntity, IHasDimension
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
	}
}
