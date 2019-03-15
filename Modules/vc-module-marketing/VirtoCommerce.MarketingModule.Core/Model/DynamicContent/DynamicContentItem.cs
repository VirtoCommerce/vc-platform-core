using System.Collections.Generic;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.MarketingModule.Core.Model
{
    public class DynamicContentItem : DynamicContentListEntry, IsHasFolder, IHasDynamicProperties
    {
        public string ContentType { get; set; }

        #region IHasFolder Members
        public string FolderId { get; set; }
        public DynamicContentFolder Folder { get; set; }
        #endregion

        #region IHasDynamicProperties Members
        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }
        #endregion
    }
}
