using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Assets
{

    public abstract class BlobEntry : AuditableEntity
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string RelativeUrl { get; set; }
    }
}
