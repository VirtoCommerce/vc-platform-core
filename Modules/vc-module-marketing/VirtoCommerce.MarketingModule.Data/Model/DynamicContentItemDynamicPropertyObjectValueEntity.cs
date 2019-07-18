using System;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class DynamicContentItemDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity, ICloneable
    {
        public virtual DynamicContentItemEntity DynamicContentItem { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as DynamicContentItemDynamicPropertyObjectValueEntity;

            if (DynamicContentItem != null)
            {
                result.DynamicContentItem = DynamicContentItem.Clone() as DynamicContentItemEntity;
            }

            return result;
        }

        #endregion
    }
}
