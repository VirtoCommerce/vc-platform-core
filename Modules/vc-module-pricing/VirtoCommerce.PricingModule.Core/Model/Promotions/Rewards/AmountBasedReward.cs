using System;
using System.Linq;

namespace VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards
{
    public abstract class AmountBasedReward : PromotionReward
    {
        protected AmountBasedReward()
        {
        }

        protected AmountBasedReward(AmountBasedReward other)
            : base(other)
        {
            AmountType = other.AmountType;
            Amount = other.Amount;
            Quantity = other.Quantity;
            MaxLimit = other.MaxLimit;
        }

        public RewardAmountType AmountType { get; set; }
        public decimal Amount { get; set; }
        public decimal MaxLimit { get; set; }
        public int Quantity { get; set; }

        public virtual decimal GetRewardAmount(decimal price, int quantity)
        {
            var result = Amount;
            if (AmountType == RewardAmountType.Relative)
            {
                result = price * Amount * 0.01m;
                if (MaxLimit > 0)
                {
                    result = Math.Min(MaxLimit, result);
                }
            }
            if (Quantity > 0)
            {
                //Need to allocate adjustment between given quantities
                result = result * Math.Min(Quantity, quantity);
                //TODO: need allocate more rightly between each quantities
                result = result.Allocate(quantity).FirstOrDefault();
            }
            return result;
        }

        public decimal CalculateDiscountAmount(decimal price, int quantity = 1)
        {
            var retVal = Amount;
            if (AmountType == RewardAmountType.Relative)
            {
                var amount = Amount;
                if (MaxLimit > 0)
                {
                    amount = Math.Min(MaxLimit, Amount);
                }
                retVal = Math.Floor(price * amount) * Math.Min(quantity, Quantity == 0 ? quantity : Quantity);
            }
            return FinanceRound(retVal);
        }

        private static decimal FinanceRound(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}
