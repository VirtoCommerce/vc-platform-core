using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    /// <summary>
    /// Represents the prototype for promotion tree <see cref="PromotionConditionAndRewardTree"/> contains the list of available conditions for building a tree in designer
    /// </summary>
    public class PromotionConditionAndRewardTreePrototype : ConditionTree
    {
        public PromotionConditionAndRewardTreePrototype()
        {
            var blockCustomer = new BlockCustomerCondition()
                    .WithAvailConditions(
                        new ConditionIsRegisteredUser(),
                        new ConditionIsEveryone(),
                        new ConditionIsFirstTimeBuyer(),
                        new UserGroupsContainsCondition()
                     );
            var blockCatalog = new BlockCatalogCondition()
                    .WithAvailConditions(
                        new ConditionCategoryIs(),
                        new ConditionCodeContains(),
                        new ConditionCurrencyIs(),
                        new ConditionEntryIs(),
                        new ConditionInStockQuantity()
                     );
            var blockCart = new BlockCartCondition()
                    .WithAvailConditions(
                        new ConditionAtNumItemsInCart(),
                        new ConditionAtNumItemsInCategoryAreInCart(),
                        new ConditionAtNumItemsOfEntryAreInCart(),
                        new ConditionCartSubtotalLeast()
                     );
            var blockReward = new BlockReward()
                    .WithAvailConditions(
                      new RewardCartGetOfAbsSubtotal(),
                      new RewardCartGetOfRelSubtotal(),
                      new RewardItemGetFreeNumItemOfProduct(),
                      new RewardItemGetOfAbs(),
                      new RewardItemGetOfAbsForNum(),
                      new RewardItemGetOfRel(),
                      new RewardItemGetOfRelForNum(),
                      new RewardItemGiftNumItem(),
                      new RewardShippingGetOfAbsShippingMethod(),
                      new RewardShippingGetOfRelShippingMethod(),
                      new RewardPaymentGetOfAbs(),
                      new RewardPaymentGetOfRel(),
                      new RewardItemForEveryNumInGetOfRel(),
                      new RewardItemForEveryNumOtherItemInGetOfRel()
                    );
            WithAvailConditions(
                blockCustomer,
                blockCatalog,
                blockCart,
                blockReward
           );
            WithChildrens(
                blockCustomer,
                blockCatalog,
                blockCart,
                blockReward
                );
        }
    }
}
