using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;

namespace VirtoCommerce.MarketingModule.Core.Model
{
    public class DynamicContentPlace : DynamicContentListEntry, IsHasFolder
    {
        #region IHasFolder Members
        public string FolderId { get; set; }
        public DynamicContentFolder Folder { get; set; }
        #endregion

        #region ICloneable members

        public override object Clone()
        {
            var result = base.Clone() as DynamicContentPlace;

            if (Folder != null)
            {
                result.Folder = Folder.Clone() as DynamicContentFolder;
            }

            return result;
        }

        #endregion
    }
}
