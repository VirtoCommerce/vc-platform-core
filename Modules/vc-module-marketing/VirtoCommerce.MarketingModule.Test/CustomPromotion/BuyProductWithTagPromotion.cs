using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Rewards;

namespace VirtoCommerce.MarketingModule.Test.CustomPromotion
{
    public class BuyProductWithTagPromotion : Promotion
    {
        private readonly string[] _tags;
        private readonly decimal _discountAmount;

        public BuyProductWithTagPromotion(string[] tags, decimal discountAmount)
        {
            _tags = tags;
            _discountAmount = discountAmount;
        }

        public override PromotionReward[] EvaluatePromotion(IEvaluationContext context)
        {
            var retVal = new List<PromotionReward>();
            if (context is PromotionEvaluationContext promoContext)
            {
                foreach (var entry in promoContext.PromoEntries)
                {
                    var tag = entry.Attributes != null ? entry.Attributes["tag"] : null;
                    var reward = new CatalogItemAmountReward
                    {
                        AmountType = RewardAmountType.Relative,
                        Amount = _discountAmount,
                        IsValid = !string.IsNullOrEmpty(tag) && _tags.Contains(tag),
                        ProductId = entry.ProductId,
                        Promotion = this
                    };
                }
            }
            return retVal.ToArray();
        }
    }
}
