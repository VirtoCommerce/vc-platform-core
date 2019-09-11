using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Promotions
{
    public class DynamicPromotion : Promotion, ICloneable
    {
        public DynamicPromotion()
        {
            Type = nameof(DynamicPromotion); 
        }

        /// <summary>
        /// If this flag is set to true, it allows this promotion to combine with itself.
        /// Special for case when need to return same promotion rewards for multiple coupons
        /// </summary>
        public bool IsAllowCombiningWithSelf { get; set; }

        public  ICouponSearchService CouponSearchService { get; set; }
        public IPromotionUsageSearchService PromotionUsageSearchService { get; set; }

        public PromotionConditionAndRewardTree DynamicExpression { get; set; }

        public override async Task<PromotionReward[]> EvaluatePromotionAsync(IEvaluationContext context)
        {
            var result = new List<PromotionReward>();

            if (!(context is PromotionEvaluationContext promoContext))
            {
                throw new ArgumentException("context should be PromotionEvaluationContext");
            }

            IEnumerable<Coupon> validCoupons = null;
            if (HasCoupons)
            {
                validCoupons = await FindValidCouponsAsync(promoContext.Coupons, promoContext.CustomerId);
            }
            //Check coupon
            var couponIsValid = !HasCoupons || validCoupons.Any();

            //Evaluate reward for all promoEntry in context
            foreach (var promoEntry in promoContext.PromoEntries)
            {
                //Set current context promo entry for evaluation
                promoContext.PromoEntry = promoEntry;

                foreach (var reward in DynamicExpression?.GetRewards() ?? Array.Empty<PromotionReward>())
                {
                    var clonedReward = reward.Clone() as PromotionReward;
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
                            clonedReward = clonedReward.Clone() as PromotionReward;
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
            reward.IsValid = couponIsValid && (DynamicExpression?.IsSatisfiedBy(promoContext) ?? false);

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
                    var coupons = await CouponSearchService.SearchCouponsAsync(new CouponSearchCriteria { Codes = couponCodes, PromotionId = Id });
                    foreach (var coupon in coupons.Results.OrderBy(x => x.TotalUsesCount))
                    {
                        var couponIsValid = true;
                        if (coupon.ExpirationDate != null)
                        {
                            couponIsValid = coupon.ExpirationDate > DateTime.UtcNow;
                        }
                        if (couponIsValid && coupon.MaxUsesNumber > 0)
                        {
                            var usage = await PromotionUsageSearchService.SearchUsagesAsync(new PromotionUsageSearchCriteria { PromotionId = Id, CouponCode = coupon.Code, Take = 0 });
                            couponIsValid = usage.TotalCount < coupon.MaxUsesNumber;
                        }
                        if (couponIsValid && coupon.MaxUsesPerUser > 0 && !string.IsNullOrWhiteSpace(userId))
                        {
                            var usage = await PromotionUsageSearchService.SearchUsagesAsync(new PromotionUsageSearchCriteria { PromotionId = Id, CouponCode = coupon.Code, UserId = userId, Take = int.MaxValue });
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

        #region ICloneable members

        public override object Clone()
        {
            var result = base.Clone() as DynamicPromotion;

            if (DynamicExpression != null)
            {
                result.DynamicExpression = DynamicExpression.Clone() as PromotionConditionAndRewardTree;
            }
            
            return result;
        }

        #endregion
    }
}
