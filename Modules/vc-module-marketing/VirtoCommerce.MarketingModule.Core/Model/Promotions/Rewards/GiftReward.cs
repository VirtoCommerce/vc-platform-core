namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    /// <summary>
    /// Gift
    /// </summary>
    public class GiftReward : PromotionReward
    {
    
        public string Name { get; set; }

        public string CategoryId { get; set; }

        public string ProductId { get; set; }

        public int Quantity { get; set; }

        public string MeasureUnit { get; set; }

        public string ImageUrl { get; set; }


    }
}
