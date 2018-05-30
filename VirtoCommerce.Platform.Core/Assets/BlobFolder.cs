namespace VirtoCommerce.Platform.Core.Assets
{
    public class BlobFolder : BlobObject
    {
        public BlobFolder()
        {
            Type = "folder";
        }
        public string ParentUrl { get; set; }
    }
}
