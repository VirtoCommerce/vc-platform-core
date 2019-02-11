namespace VirtoCommerce.MarketingModule.Core.Model
{
    public interface IsHasFolder
    {
        string FolderId { get; set; }
        DynamicContentFolder Folder { get; set; }
    }
}
