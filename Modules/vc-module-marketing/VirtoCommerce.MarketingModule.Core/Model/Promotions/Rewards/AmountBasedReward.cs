using System;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Extensions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public abstract class AmountBasedReward : PromotionReward
    {        
        public RewardAmountType AmountType { get; set; }
        /// <summary>
        /// Reward amount
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// The max reward amount limit (Not to exceed $S)
        /// </summary>
        public decimal MaxLimit { get; set; }
        /// <summary>
        /// The max  quantity limit (No more than Q items)
        /// </summary>
        public int Quantity { get; set; }

        //For N in every Y items
        public int ForNthQuantity { get; set; }
        public int InEveryNthQuantity { get; set; }

        /// <summary>
        ///  Get per item reward amount for given items quantity and price
        /// </summary>
        /// <param name="price">Price per item</param>
        /// <param name="quantity">Total items quantity</param>
        /// <returns></returns>
        public virtual decimal GetRewardAmount(decimal price, int quantity)
        {
            if (price < 0)
            {
                throw new ArgumentNullException($"The {nameof(price)} cannot be negative");
            }
            if (quantity < 0)
            {
                throw new ArgumentNullException($"The {nameof(quantity)} cannot be negative");
            }

            var workQuantity = quantity = Math.Max(1, quantity);
            if (ForNthQuantity > 0 && InEveryNthQuantity > 0)
            {
                workQuantity = workQuantity / InEveryNthQuantity * ForNthQuantity;
            }
            if (Quantity > 0)
            {
                workQuantity = Math.Min(Quantity, workQuantity);
            }
            var result = Amount * workQuantity;
            if (AmountType == RewardAmountType.Relative)
            {
                result = price * Amount * 0.01m * workQuantity;
            }
            var totalCost = price * quantity;
            //use total cost as MaxLimit if it explicitly not set
            var workMaxLimit = MaxLimit > 0 ? MaxLimit : totalCost;
            //Do not allow maxLimit be greater that total cost (to prevent reward amount be greater that price)
            workMaxLimit = Math.Min(workMaxLimit, totalCost);
            result = Math.Min(workMaxLimit, result);

            //TODO: need allocate more rightly between  given quantities
            result = result.Allocate(quantity).FirstOrDefault();
            return result;
        }


        [Obsolete("Use GetRewardAmount instead")]
        public decimal CalculateDiscountAmount(decimal price, int quantity = 1)
        {
            return GetRewardAmount(price, quantity);
        }
    }
}
