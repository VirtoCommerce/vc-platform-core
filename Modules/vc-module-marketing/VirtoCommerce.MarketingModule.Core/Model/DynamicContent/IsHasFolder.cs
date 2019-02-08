namespace VirtoCommerce.MarketingModule.Core.Model.DynamicContent
{
    public interface IsHasFolder
    {
        string FolderId { get; set; }
        DynamicContentFolder Folder { get; set; }
    }
}
