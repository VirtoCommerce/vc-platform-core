using System.Collections.Generic;

namespace VirtoCommerce.Tools.Models
{
    public class OutlineItem
    {
        public string Id { get; set; }
        public string SeoObjectType { get; set; }
        public IList<SeoInfo> SeoInfos { get; set; }
        public bool? HasVirtualParent { get; set; }
    }
}
