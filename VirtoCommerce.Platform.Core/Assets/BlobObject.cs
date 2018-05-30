using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Platform.Core.Assets
{

    public abstract class BlobObject
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string RelativeUrl { get; set; }
    }
}
