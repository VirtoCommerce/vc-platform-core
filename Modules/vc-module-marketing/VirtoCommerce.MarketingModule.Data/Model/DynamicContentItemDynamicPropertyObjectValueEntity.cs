using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class DynamicContentItemDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual DynamicContentItemEntity DynamicContentItem { get; set; }

        #region ICloneable members

        public override object Clone()
        {
            var result = base.Clone() as DynamicContentItemDynamicPropertyObjectValueEntity;

            if (DynamicContentItem != null)
            {
                result.DynamicContentItem = DynamicContentItem.Clone() as DynamicContentItemEntity;
            }

            return result;
        }

        #endregion
    }
}
