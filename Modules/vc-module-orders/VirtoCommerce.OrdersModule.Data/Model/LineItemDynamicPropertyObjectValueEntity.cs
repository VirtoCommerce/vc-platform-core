using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    public class LineItemDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual LineItemEntity LineItem { get; set; }
    }
}
