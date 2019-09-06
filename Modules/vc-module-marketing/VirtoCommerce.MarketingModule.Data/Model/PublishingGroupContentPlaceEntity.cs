using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class PublishingGroupContentPlaceEntity : AuditableEntity
    {
        #region Navigation Properties

        public string DynamicContentPublishingGroupId { get; set; }
        public virtual DynamicContentPublishingGroupEntity PublishingGroup { get; set; }

        public string DynamicContentPlaceId { get; set; }
        public virtual DynamicContentPlaceEntity ContentPlace { get; set; }

        #endregion
    }
}
