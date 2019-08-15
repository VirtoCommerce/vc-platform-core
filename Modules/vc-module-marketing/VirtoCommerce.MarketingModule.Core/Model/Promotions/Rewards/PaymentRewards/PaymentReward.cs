namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    /// <summary>
    /// Payment reward
    /// </summary>
    public class PaymentReward : AmountBasedReward
    {
        public string PaymentMethod { get; set; }
    }
}
