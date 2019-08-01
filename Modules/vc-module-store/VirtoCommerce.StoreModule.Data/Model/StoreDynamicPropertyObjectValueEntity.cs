using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.StoreModule.Data.Model
{
    public class StoreDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual StoreEntity Store { get; set; }
    }
}
