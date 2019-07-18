using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class PublishingGroupContentPlaceEntity : AuditableEntity, ICloneable
    {
        #region Navigation Properties

        public string DynamicContentPublishingGroupId { get; set; }
        public virtual DynamicContentPublishingGroupEntity PublishingGroup { get; set; }

        public string DynamicContentPlaceId { get; set; }
        public virtual DynamicContentPlaceEntity ContentPlace { get; set; }

        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as PublishingGroupContentPlaceEntity;

            if (PublishingGroup != null)
            {
                result.PublishingGroup = PublishingGroup.Clone() as DynamicContentPublishingGroupEntity;
            }

            if (ContentPlace != null)
            {
                result.ContentPlace = ContentPlace.Clone() as DynamicContentPlaceEntity;
            }

            return result;
        }

        #endregion
    }
}
