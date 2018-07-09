using System;

namespace VirtoCommerce.Tools.Models
{
    public class SeoInfo
    {
        public string Name { get; set; }
        public string SemanticUrl { get; set; }
        public string PageTitle { get; set; }
        public string MetaDescription { get; set; }
        public string ImageAltDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string StoreId { get; set; }
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public bool? IsActive { get; set; }
        public string LanguageCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string Id { get; set; }
    }
}
