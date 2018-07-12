using System.Collections.Generic;

namespace VirtoCommerce.Tools.Models
{
    public class Store
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string SecureUrl { get; set; }
        public string Catalog { get; set; }
        public string DefaultLanguage { get; set; }
        public SeoLinksType SeoLinksType { get; set; }
        public IList<string> Languages { get; set; }
    }
}
