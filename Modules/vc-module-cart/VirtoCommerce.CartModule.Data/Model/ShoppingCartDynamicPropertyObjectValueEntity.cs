using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class ShoppingCartDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual ShoppingCartEntity ShoppingCart { get; set; }
    }
}
