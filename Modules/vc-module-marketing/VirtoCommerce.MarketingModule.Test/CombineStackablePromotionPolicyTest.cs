using System.Collections.Generic;
using System.Linq;
using Moq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Promotions;
using VirtoCommerce.MarketingModule.Data.Services;
using VirtoCommerce.MarketingModule.Test.CustomPromotion;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace VirtoCommerce.MarketingModule.Test
{
    [Trait("Category", "CI")]
    public class CombineStackablePromotionPolicyTest
    {
        [Fact]
        public void EvaluateRewards_CombineByPriorityOrder()
        {
            //Arrange            
            var evalPolicy = GetPromotionEvaluationPolicy(GetPromotions("FedEx Get 50% Off", "FedEx Get 30% Off", "ProductA and ProductB Get 2 With 50% Off", "Get ProductA With 25$ Off"));
            var productA = new ProductPromoEntry { ProductId = "ProductA", Price = 100, Quantity = 1 };
            var productB = new ProductPromoEntry { ProductId = "ProductB", Price = 100, Quantity = 3 };
            var context = new PromotionEvaluationContext
            {
                ShipmentMethodCode = "FedEx",
                ShipmentMethodPrice = 100,
                PromoEntries = new[] { productA, productB }
            };
            //Act
            var rewards = evalPolicy.EvaluatePromotionAsync(context).GetAwaiter().GetResult().Rewards;

            //Assert
            Assert.Equal(5, rewards.Count());
            Assert.Equal(35m, context.ShipmentMethodPrice);
            Assert.Equal(37.5m, productA.Price);
            Assert.Equal(66.66m, productB.Price);
        }

        [Fact]
        public void EvaluateRewards_OnlySingleExlusivePromotion()
        {
            //Arrange            
            var evalPolicy = GetPromotionEvaluationPolicy(TestPromotions);
            var productA = new ProductPromoEntry { ProductId = "ProductA", Price = 100, Quantity = 1 };
            var productB = new ProductPromoEntry { ProductId = "ProductB", Price = 100, Quantity = 1 };
            var context = new PromotionEvaluationContext
            {
                ShipmentMethodCode = "FedEx",
                ShipmentMethodPrice = 100,
                PromoEntries = new[] { productA, productB }
            };
            //Act
            var rewards = evalPolicy.EvaluatePromotionAsync(context).GetAwaiter().GetResult().Rewards;

            //Assert
            Assert.Single(rewards);
            Assert.Equal("Exclusive ProductB Get 10$ Off", rewards.Single().Promotion.Id);
        }

        [Fact]
        public void EvaluateRewards_SkipRewardsMakingPriceNegative()
        {
            //Arrange            
            var evalPolicy = GetPromotionEvaluationPolicy(GetPromotions("Get ProductA Free", "Get ProductA With 25$ Off"));
            var productA = new ProductPromoEntry { ProductId = "ProductA", Price = 100, Quantity = 1 };
            var context = new PromotionEvaluationContext
            {
                PromoEntries = new[] { productA }
            };
            //Act
            var rewards = evalPolicy.EvaluatePromotionAsync(context).GetAwaiter().GetResult().Rewards;

            //Assert
            Assert.Single(rewards);
            Assert.Equal("Get ProductA Free", rewards.Single().Promotion.Id);
            Assert.Equal(0, productA.Price);
        }

        [Fact]
        public void EvaluateRewards_ShippingMethodNotSpecified_Counted()
        {
            //Arrange            
            var evalPolicy = GetPromotionEvaluationPolicy(GetPromotions("Any shipment 50% Off"));
            var productA = new ProductPromoEntry { ProductId = "ProductA", Price = 100, Quantity = 1 };
            var context = new PromotionEvaluationContext
            {
                ShipmentMethodCode = "FedEx",
                ShipmentMethodPrice = 100,
                PromoEntries = new[] { productA }
            };
            //Act
            var rewards = evalPolicy.EvaluatePromotionAsync(context).GetAwaiter().GetResult().Rewards;

            //Assert
            Assert.Equal(1, rewards.Count);
            Assert.Equal(50m, context.ShipmentMethodPrice);
            Assert.Equal(100m, productA.Price);
        }

        [Fact]
        public void EvaluateRewards_BuyProductWithTag_Counted()
        {
            //Arrange 
            var evalPolicy = GetPromotionEvaluationPolicy(new List<BuyProductWithTagPromotion>()
            {
                new BuyProductWithTagPromotion(new [] {"tag1"}, 10)
            });
            var productA = new ProductPromoEntry { ProductId = "ProductA", Price = 100, Quantity = 1, Attributes = new Dictionary<string, string> { { "tag", "tag1" } } };
            var context = new PromotionEvaluationContext
            {
                PromoEntries = new[] { productA }
            };

            //Act
            var rewards = evalPolicy.EvaluatePromotionAsync(context).GetAwaiter().GetResult().Rewards;

            //Assert
            Assert.Equal(1, rewards.Count);
        }

        [Fact]
        public void EvaluateRewards_DynamicPromotions()
        {
            //Arrange 
            var couponServiceMock = new Mock<ICouponService>();
            var promotionUsageMock = new Mock<IPromotionUsageService>();

            var evalPolicy = GetPromotionEvaluationPolicy(new List<Promotion> { new DynamicPromotion(couponServiceMock.Object, promotionUsageMock.Object)
            {
                PredicateSerialized = "[{\"All\":false,\"Not\":false,\"AvailableChildren\":[],\"Children\":[{\"AvailableChildren\":[],\"Children\":[],\"Id\":\"ConditionIsRegisteredUser\"}],\"Id\":\"BlockCustomerCondition\"},{\"All\":false,\"Not\":false,\"AvailableChildren\":[],\"Children\":[],\"Id\":\"BlockCartCondition\"},{\"All\":false,\"Not\":false,\"AvailableChildren\":[],\"Children\":[],\"Id\":\"BlockCatalogCondition\"}]",
                RewardsSerialized = "[{\"AmountType\":0,\"Amount\":2.0,\"MaxLimit\":0.0,\"Quantity\":0,\"ForNthQuantity\":0,\"InEveryNthQuantity\":0,\"Id\":\"CartSubtotalReward\",\"IsValid\":false,\"Description\":null,\"CouponAmount\":0.0,\"Coupon\":null,\"CouponMinOrderAmount\":null,\"Promotion\":null}]",

            } });
            var context = new PromotionEvaluationContext() { IsRegisteredUser = true, IsEveryone = true, Currency = "usd", PromoEntries = new List<ProductPromoEntry> { new ProductPromoEntry() { ProductId = "1" } } };

            RegisterConditionRewards();

            //Act
            var rewards = evalPolicy.EvaluatePromotionAsync(context).GetAwaiter().GetResult().Rewards;

            //Assert
            Assert.Equal(1, rewards.Count);
        }

        private static IMarketingPromoEvaluator GetPromotionEvaluationPolicy(IEnumerable<Promotion> promotions)
        {
            var result = new GenericSearchResult<Promotion>
            {
                Results = promotions.ToList()
            };
            var promoSearchServiceMock = new Moq.Mock<IPromotionSearchService>();
            promoSearchServiceMock.Setup(x => x.SearchPromotionsAsync(It.IsAny<PromotionSearchCriteria>())).ReturnsAsync(result);

            return new CombineStackablePromotionPolicy(promoSearchServiceMock.Object);
        }


        private static IEnumerable<Promotion> TestPromotions
        {
            get
            {
                yield return new MockPromotion
                {
                    Id = "FedEx Get 50% Off",
                    Rewards = new[]
                    {
                        new ShipmentReward { ShippingMethod = "FedEx", Amount = 50, AmountType = RewardAmountType.Relative, IsValid = true }
                    },
                    Priority = 1,
                    IsExclusive = false
                };
                yield return new MockPromotion
                {
                    Id = "FedEx Get 30% Off",
                    Rewards = new[]
                   {
                        new ShipmentReward { ShippingMethod = "FedEx", Amount = 30, AmountType = RewardAmountType.Relative, IsValid = true  }
                    },
                    Priority = 2,
                    IsExclusive = false
                };
                yield return new MockPromotion
                {
                    Id = "Any shipment 50% Off",
                    Rewards = new[]
                   {
                        new ShipmentReward { ShippingMethod = null, Amount = 50, AmountType = RewardAmountType.Relative, IsValid = true  }
                    },
                    Priority = 2,
                    IsExclusive = false
                };
                yield return new MockPromotion
                {
                    Id = "Exclusive ProductB Get 10$ Off",
                    Rewards = new[]
                   {
                        new CatalogItemAmountReward { ProductId = "ProductB", Amount = 10, AmountType = RewardAmountType.Absolute, IsValid = true }
                    },
                    Priority = 10,
                    IsExclusive = true
                };
                yield return new MockPromotion
                {
                    Id = "Get ProductA Free",
                    Rewards = new[]
                    {
                       new CatalogItemAmountReward { ProductId = "ProductA", Amount = 100, AmountType = RewardAmountType.Relative, IsValid = true },
                    },
                    Priority = 100,
                    IsExclusive = false
                };
                yield return new MockPromotion
                {
                    Id = "Get ProductA With 25$ Off",
                    Rewards = new[]
                    {
                       new CatalogItemAmountReward { ProductId = "ProductA", Amount = 25, AmountType = RewardAmountType.Absolute, IsValid = true },
                    },
                    Priority = 80,
                    IsExclusive = false
                };
                yield return new MockPromotion
                {
                    Id = "ProductA and ProductB Get 2 With 50% Off",
                    Rewards = new[]
                    {
                       new CatalogItemAmountReward { ProductId = "ProductA", Amount = 50, Quantity = 2, AmountType = RewardAmountType.Relative, IsValid = true },
                       new CatalogItemAmountReward { ProductId = "ProductB", Amount = 50, Quantity = 2, AmountType = RewardAmountType.Relative, IsValid = true}
                    },
                    Priority = 15,
                    IsExclusive = false
                };
                yield return new MockPromotion
                {
                    Id = "Buy Order with 55% Off",
                    Rewards = new[]
                    {
                       new CartSubtotalReward {  Amount = 55, IsValid = true }
                    },
                    Priority = 20,
                    IsExclusive = false
                };
                yield return new MockPromotion
                {
                    Id = "Get Gift",
                    Rewards = new[]
                    {
                       new GiftReward {  ProductId = "ProductA", IsValid = true }
                    },
                    Priority = 0,
                    IsExclusive = false
                };
            }
        }

        private static IEnumerable<Promotion> GetPromotions(params string[] ids)
        {
            return TestPromotions.Where(x => ids.Contains(x.Id));
        }


        private static void RegisterConditionRewards()
        {
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<PromotionConditionReward>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<BlockConditionAndOr>();

            AbstractTypeFactory<IConditionRewardTree>.RegisterType<BlockCustomerCondition>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionIsRegisteredUser>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionIsEveryone>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionIsFirstTimeBuyer>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<UserGroupsContainsCondition>();

            AbstractTypeFactory<IConditionRewardTree>.RegisterType<BlockCatalogCondition>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionAtCartItemExtendedTotal>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionAtNumItemsInCart>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionAtNumItemsInCategoryAreInCart>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionAtNumItemsOfEntryAreInCart>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionCartSubtotalLeast>();

            AbstractTypeFactory<IConditionRewardTree>.RegisterType<BlockCartCondition>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionCategoryIs>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionCodeContains>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionCurrencyIs>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionEntryIs>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionInStockQuantity>();

            AbstractTypeFactory<IConditionRewardTree>.RegisterType<BlockReward>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardCartGetOfAbsSubtotal>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardCartGetOfRelSubtotal>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemGetFreeNumItemOfProduct>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemGetOfAbs>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemGetOfAbsForNum>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemGetOfRel>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemGetOfRelForNum>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemGiftNumItem>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardShippingGetOfAbsShippingMethod>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardShippingGetOfRelShippingMethod>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardPaymentGetOfAbs>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardPaymentGetOfRel>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemForEveryNumInGetOfRel>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemForEveryNumOtherItemInGetOfRel>();

            AbstractTypeFactory<PromotionReward>.RegisterType<GiftReward>();
            AbstractTypeFactory<PromotionReward>.RegisterType<CartSubtotalReward>();
            AbstractTypeFactory<PromotionReward>.RegisterType<CatalogItemAmountReward>();
            AbstractTypeFactory<PromotionReward>.RegisterType<PaymentReward>();
            AbstractTypeFactory<PromotionReward>.RegisterType<ShipmentReward>();
            AbstractTypeFactory<PromotionReward>.RegisterType<SpecialOfferReward>();
        }

    }

    internal class MockPromotion : Promotion
    {
        public IEnumerable<PromotionReward> Rewards { get; set; }

        public override PromotionReward[] EvaluatePromotion(IEvaluationContext context)
        {
            foreach (var reward in Rewards)
            {
                reward.Promotion = this;
            }
            return Rewards.ToArray();
        }
    }
}
