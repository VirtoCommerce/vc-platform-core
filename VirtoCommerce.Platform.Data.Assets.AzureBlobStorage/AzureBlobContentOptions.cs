using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;

namespace VirtoCommerce.Platform.Data.Assets.AzureBlobStorage
{
    public class AzureBlobContentOptions
    {
        public string ConnectionString { get; set; }
        public string CdnUrl { get; set; }
        public BlobRequestOptions BlobRequestOptions { get; set; } = new BlobRequestOptions();
    }
}
