using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrderModule.Core.Model
{
	public class ShipmentItem : AuditableEntity
	{
		public ShipmentItem()
		{
		}

		public ShipmentItem(LineItem lineItem)
		{
			LineItem = lineItem;
			LineItemId = lineItem.Id;

			Quantity = lineItem.Quantity;
		}

		public string LineItemId { get; set; }
		public LineItem LineItem { get; set; }

		public string BarCode { get; set; }

		public int Quantity { get; set; }
	
	}
}
