using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ContentModule.Core.Model
{
    /// <summary>
    /// Represent content file
    /// </summary>
    public class ContentFile : ContentItem
    {
        public ContentFile()
            : base("blob")
        {
        }

        public string MimeType { get; set; }
        public string Size { get; set; }
    }
}
