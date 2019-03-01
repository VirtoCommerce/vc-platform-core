﻿namespace VirtoCommerce.CatalogModule.Web.Model
{
    /// <summary>
    /// Asset containing any content.
    /// </summary>
	public class Asset : AssetBase
    {
        public Asset()
        {
            TypeId = "asset";
        }
        public long Size { get; set; }

        public string ReadableSize { get; set; }
        
        public string MimeType { get; set; }
    }
}
