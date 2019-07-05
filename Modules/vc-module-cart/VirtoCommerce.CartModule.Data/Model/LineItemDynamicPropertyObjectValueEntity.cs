using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class LineItemDynamicPropertyObjectValueEntity: DynamicPropertyObjectValueEntity
    {
        public virtual LineItemEntity LineItem { get; set; }
    }
}
