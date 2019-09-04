namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public class CatalogItemAmountReward : AmountBasedReward
    {
       
        /// <summary>
        /// Target reward product
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// Conditional product
        /// For N items of entry ProductId  in every Y items of entry ConditionalProductId get %X off
        /// </summary>
        public string ConditionalProductId { get; set; }


    }
}
