using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;

namespace VirtoCommerce.MarketingModule.Core.Model
{
    public class DynamicContentFolder : DynamicContentListEntry
    {
        public string Path => ParentFolder == null ? Name : ParentFolder.Path + "\\" + Name;
        public string Outline => ParentFolder == null ? Id : ParentFolder.Outline + ";" + Id;

        public string ParentFolderId { get; set; }
        public DynamicContentFolder ParentFolder { get; set; }

        public override string ObjectType => nameof(DynamicContentFolder);
    }
}
