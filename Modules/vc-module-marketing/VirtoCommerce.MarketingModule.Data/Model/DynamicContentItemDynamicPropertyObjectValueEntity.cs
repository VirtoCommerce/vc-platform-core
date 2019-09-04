using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class DynamicContentItemDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual DynamicContentItemEntity DynamicContentItem { get; set; }
    }
}
