using System;

namespace VirtoCommerce.MarketingModule.Web.Model
{
    public class ImportRequest
    {
        public string FileUrl { get; set; }

        public string Delimiter { get; set; }

        public string PromotionId { get; set; }

        public DateTime? ExpirationDate { get; set; }
    }
}