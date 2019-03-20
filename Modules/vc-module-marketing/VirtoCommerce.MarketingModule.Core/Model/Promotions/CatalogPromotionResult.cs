
namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public abstract class CatalogPromotionResult
    {
        public CatalogPromotionResult(Promotion promo)
        {
            CatalogPromotionId = promo.Name;
            Description = promo.Description;
        }
        public bool IsValid { get; set; }

        public string CatalogPromotionId { get; set; }
        public string Description { get; set; }
    }
}
