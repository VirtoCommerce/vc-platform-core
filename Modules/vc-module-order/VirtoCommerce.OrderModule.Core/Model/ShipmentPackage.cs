using System.Collections.Generic;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrderModule.Core.Model
{
	public class ShipmentPackage : AuditableEntity, IHaveDimension
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
