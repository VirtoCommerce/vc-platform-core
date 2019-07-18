using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class PublishingGroupContentItemEntity : AuditableEntity, ICloneable
    {
        #region Navigation Properties

        public string DynamicContentPublishingGroupId { get; set; }
        public virtual DynamicContentPublishingGroupEntity PublishingGroup { get; set; }

        public string DynamicContentItemId { get; set; }
        public virtual DynamicContentItemEntity ContentItem { get; set; }

        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as PublishingGroupContentItemEntity;

            if (PublishingGroup != null)
            {
                result.PublishingGroup = PublishingGroup.Clone() as DynamicContentPublishingGroupEntity;
            }

            if (ContentItem != null)
            {
                result.ContentItem = ContentItem.Clone() as DynamicContentItemEntity;
            }

            return result;
        }

        #endregion
    }
}
