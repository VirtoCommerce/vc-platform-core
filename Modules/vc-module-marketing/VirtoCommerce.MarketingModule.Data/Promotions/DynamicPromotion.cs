using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Promotions
{
    public class DynamicPromotion : Promotion
    {
        private readonly ICouponService _couponService;
        private readonly IPromotionUsageService _usageService;

        private Condition[] _conditions;
        private PromotionReward[] _rewards;

        public DynamicPromotion(ICouponService couponService, IPromotionUsageService usageService)
        {
            _couponService = couponService;
            _usageService = usageService;
        }

        /// <summary>
        /// If this flag is set to true, it allows this promotion to combine with itself.
        /// Special for case when need to return same promotion rewards for multiple coupons  
        /// </summary>
        public bool IsAllowCombiningWithSelf { get; set; }

        public string PredicateSerialized { get; set; }
        public string PredicateVisualTreeSerialized { get; set; }
        public string RewardsSerialized { get; set; }

        protected Condition[] Conditions => _conditions ?? (_conditions = JsonConvert.DeserializeObject<Condition[]>(PredicateSerialized, new PromotionConditionRewardJsonConverter()));

        protected PromotionReward[] Rewards => _rewards ?? (_rewards = JsonConvert.DeserializeObject<PromotionReward[]>(RewardsSerialized, new PromotionConditionRewardJsonConverter()));

        public override PromotionReward[] EvaluatePromotion(IEvaluationContext context)
        {
            var result = new List<PromotionReward>();

            if (!(context is PromotionEvaluationContext promoContext))
            {
                throw new ArgumentException("context should be PromotionEvaluationContext");
            }

            IEnumerable<Coupon> validCoupons = null;
            if (HasCoupons)
            {
                validCoupons = FindValidCouponsAsync(promoContext.Coupons, promoContext.CustomerId).GetAwaiter().GetResult();
            }
            //Check coupon
            var couponIsValid = !HasCoupons || validCoupons.Any();

            //Evaluate reward for all promoEntry in context
            foreach (var promoEntry in promoContext.PromoEntries)
            {
                //Set current context promo entry for evaluation
                promoContext.PromoEntry = promoEntry;
                foreach (var reward in Rewards)
                {
                    var clonedReward = reward.Clone();
                    EvaluateReward(promoContext, couponIsValid, clonedReward);
                    //Add coupon to reward only for case when promotion contains associated coupons
                    if (!validCoupons.IsNullOrEmpty())
                    {
                        //Need to return promotion rewards for each valid coupon if promotion IsAllowCombiningWithSelf flag set
                        foreach (var validCoupon in IsAllowCombiningWithSelf ? validCoupons : validCoupons.Take(1))
                        {
                            clonedReward.Promotion = this;
                            clonedReward.Coupon = validCoupon.Code;
                            result.Add(clonedReward);
                            //Clone reward for next iteration
                            clonedReward = clonedReward.Clone();
                        }
                    }
                    else
                    {
                        result.Add(clonedReward);
                    }
                }
            }
            return result.ToArray();
        }


        protected virtual void EvaluateReward(PromotionEvaluationContext promoContext, bool couponIsValid, PromotionReward reward)
        {
            reward.Promotion = this;
            reward.IsValid = couponIsValid && Conditions.All(c => c.Evaluate(promoContext));

            //Set productId for catalog item reward
            if (reward is CatalogItemAmountReward catalogItemReward && catalogItemReward.ProductId == null)
            {
                catalogItemReward.ProductId = promoContext.PromoEntry.ProductId;
            }
        }

        protected virtual async Task<IEnumerable<Coupon>> FindValidCouponsAsync(ICollection<string> couponCodes, string userId)
        {
            var result = new List<Coupon>();
            if (!couponCodes.IsNullOrEmpty())
            {
                //Remove empty codes from input list
                couponCodes = couponCodes.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (!couponCodes.IsNullOrEmpty())
                {
                    var coupons = await _couponService.SearchCouponsAsync(new CouponSearchCriteria { Codes = couponCodes, PromotionId = Id });
                    foreach (var coupon in coupons.Results.OrderBy(x => x.TotalUsesCount))
                    {
                        var couponIsValid = true;
                        if (coupon.ExpirationDate != null)
                        {
                            couponIsValid = coupon.ExpirationDate > DateTime.UtcNow;
                        }
                        if (couponIsValid && coupon.MaxUsesNumber > 0)
                        {
                            var usage = await _usageService.SearchUsagesAsync(new PromotionUsageSearchCriteria { PromotionId = Id, CouponCode = coupon.Code, Take = 0 });
                            couponIsValid = usage.TotalCount <= coupon.MaxUsesNumber;
                        }
                        if (couponIsValid && coupon.MaxUsesPerUser > 0 && !string.IsNullOrWhiteSpace(userId))
                        {
                            var usage = await _usageService.SearchUsagesAsync(new PromotionUsageSearchCriteria { PromotionId = Id, CouponCode = coupon.Code, UserId = userId, Take = int.MaxValue });
                            couponIsValid = usage.TotalCount < coupon.MaxUsesPerUser;
                        }
                        if (couponIsValid)
                        {
                            result.Add(coupon);
                        }
                    }
                }
            }
            return result;
        }
    }
}
