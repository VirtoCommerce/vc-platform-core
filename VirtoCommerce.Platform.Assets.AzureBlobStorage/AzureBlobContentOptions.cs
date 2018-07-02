using Microsoft.WindowsAzure.Storage.Blob;

namespace VirtoCommerce.Platform.Assets.AzureBlobStorage
{
    public class AzureBlobContentOptions
    {
        public string ConnectionString { get; set; }
        public string CdnUrl { get; set; }
        public string RootPath { get; set; }
        public BlobRequestOptions BlobRequestOptions { get; set; } = new BlobRequestOptions();
    }
}
