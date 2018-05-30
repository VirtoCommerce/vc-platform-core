using System;

namespace VirtoCommerce.Platform.Core.Assets
{
    public class BlobInfo : BlobObject
    {
        public BlobInfo()
        {
            Type = "blob";
        }
        /// <summary>
        /// Relative url
        /// </summary>
		public string Key { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}
