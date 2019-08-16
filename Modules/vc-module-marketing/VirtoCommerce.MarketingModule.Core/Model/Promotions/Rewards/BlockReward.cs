using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.Common;

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
                yield return AbstractTypeFactory<RewardCartGetOfAbsSubtotal>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardCartGetOfRelSubtotal>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardItemGetFreeNumItemOfProduct>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardItemGetOfAbs>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardItemGetOfAbsForNum>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardItemGetOfRel>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardItemGetOfRelForNum>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardItemGiftNumItem>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardShippingGetOfAbsShippingMethod>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardShippingGetOfRelShippingMethod>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardPaymentGetOfAbs>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardPaymentGetOfRel>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardItemForEveryNumInGetOfRel>.TryCreateInstance();
                yield return AbstractTypeFactory<RewardItemForEveryNumOtherItemInGetOfRel>.TryCreateInstance();
            }
        }
    }
}
