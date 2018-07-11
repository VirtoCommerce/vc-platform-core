using System.Collections.Generic;

namespace VirtoCommerce.Tools.Models
{
    public class UrlBuilderContext
    {
        public string CurrentUrl { get; set; }
        public string CurrentLanguage { get; set; }
        public Store CurrentStore { get; set; }
        public IList<Store> AllStores { get; set; }
    }
}
