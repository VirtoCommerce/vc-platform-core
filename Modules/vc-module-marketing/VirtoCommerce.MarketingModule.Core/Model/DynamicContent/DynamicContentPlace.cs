using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;

namespace VirtoCommerce.MarketingModule.Core.Model
{
    public class DynamicContentPlace : DynamicContentListEntry, IsHasFolder
    {
        #region IHasFolder Members
        public string FolderId { get; set; }
        public DynamicContentFolder Folder { get; set; }
        #endregion
    }
}
