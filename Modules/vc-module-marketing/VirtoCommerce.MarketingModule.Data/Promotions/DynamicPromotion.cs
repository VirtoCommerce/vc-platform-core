using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Rewards;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Serialization;

namespace VirtoCommerce.MarketingModule.Data.Promotions
{
    public class DynamicPromotion : Promotion
    {
        private readonly ICouponService _couponService;
        private readonly IPromotionUsageService _usageService;
        public DynamicPromotion(IExpressionSerializer expressionSerializer, ICouponService couponService, IPromotionUsageService usageService)
        {
            ExpressionSerializer = expressionSerializer;
            _couponService = couponService;
            _usageService = usageService;
        }
        private Func<IEvaluationContext, bool> _condition;
        private PromotionReward[] _rewards;

        /// <summary>
        /// If this flag is set to true, it allows this promotion to combine with itself.
        /// Special for case when need to return same promotion rewards for multiple coupons  
        /// </summary>
        public bool IsAllowCombiningWithSelf { get; set; }

        protected IExpressionSerializer ExpressionSerializer { get; set; }

        public string PredicateSerialized { get; set; }
        public string PredicateVisualTreeSerialized { get; set; }
        public string RewardsSerialized { get; set; }

        protected Func<IEvaluationContext, bool> Condition => _condition ?? (_condition = ExpressionSerializer.DeserializeExpression<Func<IEvaluationContext, bool>>(PredicateSerialized));
        protected PromotionReward[] Rewards => _rewards ?? (_rewards = JsonConvert.DeserializeObject<PromotionReward[]>(RewardsSerialized, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }));

        public override PromotionReward[] EvaluatePromotion(IEvaluationContext context)
        {
            var result = new List<PromotionReward>();

            var promoContext = context as PromotionEvaluationContext;
            if (promoContext == null)
            {
                throw new ArgumentException("context should be PromotionEvaluationContext");
            }

            IEnumerable<Coupon> validCoupons = null;
            if (HasCoupons)
            {
                validCoupons = FindValidCoupons(promoContext.Coupons, promoContext.CustomerId);
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
            reward.IsValid = couponIsValid && Condition(promoContext);

            //Set productId for catalog item reward
            if (reward is CatalogItemAmountReward catalogItemReward && catalogItemReward.ProductId == null)
            {
                catalogItemReward.ProductId = promoContext.PromoEntry.ProductId;
            }
        }

        //Leave this method for back compatibility
        [Obsolete]
        protected virtual bool CheckCouponIsValid(string couponCode)
        {
            return FindValidCoupons(new[] { couponCode }, null).Any();
        }

        protected virtual IEnumerable<Coupon> FindValidCoupons(ICollection<string> couponCodes, string userId)
        {
            var result = new List<Coupon>();
            if (!couponCodes.IsNullOrEmpty())
            {
                //Remove empty codes from input list
                couponCodes = couponCodes.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (!couponCodes.IsNullOrEmpty())
                {
                    var coupons = _couponService.SearchCoupons(new CouponSearchCriteria { Codes = couponCodes, PromotionId = Id }).Results.OrderBy(x => x.TotalUsesCount);
                    foreach (var coupon in coupons)
                    {
                        var couponIsValid = true;
                        if (coupon.ExpirationDate != null)
                        {
                            couponIsValid = coupon.ExpirationDate > DateTime.UtcNow;
                        }
                        if (couponIsValid && coupon.MaxUsesNumber > 0)
                        {
                            couponIsValid = _usageService.SearchUsages(new PromotionUsageSearchCriteria { PromotionId = Id, CouponCode = coupon.Code, Take = 0 }).TotalCount <= coupon.MaxUsesNumber;
                        }
                        if (couponIsValid && coupon.MaxUsesPerUser > 0 && !string.IsNullOrWhiteSpace(userId))
                        {
                            couponIsValid = _usageService.SearchUsages(new PromotionUsageSearchCriteria { PromotionId = Id, CouponCode = coupon.Code, UserId = userId, Take = int.MaxValue }).TotalCount < coupon.MaxUsesPerUser;
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
