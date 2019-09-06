namespace VirtoCommerce.Platform.Core.Common
{
    public interface ISupportSoftDeletion
    {
        bool IsDeleted { get; set; }
    }
}
