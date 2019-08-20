using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public class BlockReward : ConditionTree, IReward
    {
        #region IRewardsExpression Members

        public PromotionReward[] GetRewards()
        {
            var retVal = new PromotionReward[] { };
            if (Children != null)
            {
                retVal = Children.OfType<IReward>().SelectMany(x => x.GetRewards()).ToArray();
            }
            return retVal;
        }

        #endregion

        public override IEnumerable<IConditionTree> AvailableChildren
        {
            get
            {
                yield return new RewardCartGetOfAbsSubtotal();
                yield return new RewardCartGetOfRelSubtotal();
                yield return new RewardItemGetFreeNumItemOfProduct();
                yield return new RewardItemGetOfAbs();
                yield return new RewardItemGetOfAbsForNum();
                yield return new RewardItemGetOfRel();
                yield return new RewardItemGetOfRelForNum();
                yield return new RewardItemGiftNumItem();
                yield return new RewardShippingGetOfAbsShippingMethod();
                yield return new RewardShippingGetOfRelShippingMethod();
                yield return new RewardPaymentGetOfAbs();
                yield return new RewardPaymentGetOfRel();
                yield return new RewardItemForEveryNumInGetOfRel();
                yield return new RewardItemForEveryNumOtherItemInGetOfRel();
            }
        }
    }
}
