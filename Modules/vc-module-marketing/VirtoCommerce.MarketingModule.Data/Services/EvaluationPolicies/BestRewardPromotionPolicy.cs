using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    public class BestRewardPromotionPolicy : IMarketingPromoEvaluator
    {
        private readonly IPromotionSearchService _promotionSearchService;

        public BestRewardPromotionPolicy(IPromotionSearchService promotionSearchService)
        {
            _promotionSearchService = promotionSearchService;
        }

        public async Task<PromotionResult> EvaluatePromotionAsync(IEvaluationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!(context is PromotionEvaluationContext promoContext))
            {
                throw new ArgumentException($"{nameof(context)} type {context.GetType()} must be derived from PromotionEvaluationContext");
            }

            var promotionSearchCriteria = new PromotionSearchCriteria
            {
                OnlyActive = true,
                StoreIds = string.IsNullOrEmpty(promoContext.StoreId) ? null : new[] { promoContext.StoreId },
                Take = int.MaxValue
            };

            var promotions = await _promotionSearchService.SearchPromotionsAsync(promotionSearchCriteria);

            var result = new PromotionResult();
            var evalPromtionTasks = promotions.Results.Select(x => x.EvaluatePromotionAsync(context)).ToArray();
            await Task.WhenAll(evalPromtionTasks);
            var rewards = evalPromtionTasks.SelectMany(x => x.Result).Where(x => x.IsValid).ToArray();

            var firstOrderExclusiveReward = rewards.FirstOrDefault(x => x.Promotion.IsExclusive);
            if (firstOrderExclusiveReward != null)
            {
                //Add only rewards from exclusive promotion
                result.Rewards.AddRange(rewards.Where(x => x.Promotion == firstOrderExclusiveReward.Promotion));
            }
            else
            {
                //best shipment promotion
                var curShipmentAmount = promoContext.ShipmentMethodCode != null ? promoContext.ShipmentMethodPrice : 0m;
                var allShipmentRewards = rewards.OfType<ShipmentReward>().ToArray();
                var groupedByShippingMethodRewards = allShipmentRewards.GroupBy(x => x.ShippingMethod);
                foreach (var shipmentRewards in groupedByShippingMethodRewards)
                {
                    var bestShipmentReward = GetBestAmountReward(curShipmentAmount, shipmentRewards);
                    if (bestShipmentReward != null)
                    {
                        result.Rewards.Add(bestShipmentReward);
                    }
                }

                //best catalog item promotion
                var allItemsRewards = rewards.OfType<CatalogItemAmountReward>().ToArray();
                var groupRewards = allItemsRewards.GroupBy(x => x.ProductId).Where(x => x.Key != null);
                foreach (var groupReward in groupRewards)
                {
                    var item = promoContext.PromoEntries.FirstOrDefault(x => x.ProductId == groupReward.Key);
                    if (item != null)
                    {
                        var bestItemReward = GetBestAmountReward(item.Price, groupReward);
                        if (bestItemReward != null)
                        {
                            result.Rewards.Add(bestItemReward);
                        }
                    }
                }

                //best order promotion 
                var cartSubtotalRewards = rewards.OfType<CartSubtotalReward>().Where(x => x.IsValid).OrderByDescending(x => x.GetRewardAmount(promoContext.CartTotal, 1));
                var cartSubtotalReward = cartSubtotalRewards.FirstOrDefault(x => !string.IsNullOrEmpty(x.Coupon)) ?? cartSubtotalRewards.FirstOrDefault();
                if (cartSubtotalReward != null)
                {
                    result.Rewards.Add(cartSubtotalReward);
                }

                //Gifts
                rewards.OfType<GiftReward>().ToList().ForEach(x => result.Rewards.Add(x));

                //Special offer
                rewards.OfType<SpecialOfferReward>().ToList().ForEach(x => result.Rewards.Add(x));
            }

            return result;
        }

        protected virtual AmountBasedReward GetBestAmountReward(decimal currentAmount, IEnumerable<AmountBasedReward> reward)
        {
            AmountBasedReward retVal = null;
            var maxAbsoluteReward = reward
                .Where(y => y.AmountType == RewardAmountType.Absolute)
                .OrderByDescending(y => y.GetRewardAmount(currentAmount, 1)).FirstOrDefault();

            var maxRelativeReward = reward
                .Where(y => y.AmountType == RewardAmountType.Relative)
                .OrderByDescending(y => y.GetRewardAmount(currentAmount, 1)).FirstOrDefault();

            var absDiscountAmount = maxAbsoluteReward != null ? maxAbsoluteReward.GetRewardAmount(currentAmount, 1) : 0;
            var relDiscountAmount = maxRelativeReward != null ? currentAmount * maxRelativeReward.GetRewardAmount(currentAmount, 1) : 0;

            if (maxAbsoluteReward != null && maxRelativeReward != null)
            {
                retVal = absDiscountAmount > relDiscountAmount ? maxAbsoluteReward : maxRelativeReward;
            }
            else if (maxAbsoluteReward != null)
            {
                retVal = maxAbsoluteReward;
            }
            else if (maxRelativeReward != null)
            {
                retVal = maxRelativeReward;
            }

            return retVal;
        }
    }
}
