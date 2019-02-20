using System.Collections.Generic;
using System.Linq;
using Moq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
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

            var evalPolicy = GetPromotionEvaluationPolicy(new List<Promotion> { new DynamicPromotion(null, couponServiceMock.Object, promotionUsageMock.Object)
            {
                RewardsSerialized = "{\"$type\":\"VirtoCommerce.MarketingModule.Core.Model.Promotions.PromotionReward[], VirtoCommerce.MarketingModule.Core\",\"$values\":[{\"$type\":\"VirtoCommerce.MarketingModule.Core.Model.Promotions.CartSubtotalReward, VirtoCommerce.MarketingModule.Core\",\"AmountType\":1,\"Amount\":15.0,\"MaxLimit\":20.0,\"Quantity\":0,\"ForNthQuantity\":0,\"InEveryNthQuantity\":0,\"IsValid\":false,\"Description\":null,\"CouponAmount\":0.0,\"Coupon\":null,\"CouponMinOrderAmount\":null,\"Promotion\":null},{\"$type\":\"VirtoCommerce.MarketingModule.Core.Model.Promotions.CatalogItemAmountReward, VirtoCommerce.MarketingModule.Core\",\"ProductId\":null,\"ConditionalProductId\":null,\"AmountType\":1,\"Amount\":25.0,\"MaxLimit\":30.0,\"Quantity\":0,\"ForNthQuantity\":0,\"InEveryNthQuantity\":0,\"IsValid\":false,\"Description\":null,\"CouponAmount\":0.0,\"Coupon\":null,\"CouponMinOrderAmount\":null,\"Promotion\":null}]}",
                PredicateSerialized = "{\"AvailableChildren\":[],\"Children\":[{\"$type\":\"VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions.BlockCustomerCondition, VirtoCommerce.MarketingModule.Core\",\"All\":false,\"Not\":false,\"AvailableChildren\":[],\"Children\":[{\"$type\":\"VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions.ConditionIsEveryone, VirtoCommerce.MarketingModule.Core\",\"AvailableChildren\":[],\"Children\":[],\"Id\":\"ConditionIsEveryone\"},{\"$type\":\"VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions.ConditionIsRegisteredUser, VirtoCommerce.MarketingModule.Core\",\"AvailableChildren\":[],\"Children\":[],\"Id\":\"ConditionIsRegisteredUser\"}],\"Id\":\"BlockCustomerCondition\"},{\"$type\":\"VirtoCommerce.MarketingModule.Data.Conditions.BlockCartCondition, VirtoCommerce.MarketingModule.Core\",\"All\":false,\"Not\":false,\"AvailableChildren\":[],\"Children\":[{\"$type\":\"VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions.ConditionCartSubtotalLeast, VirtoCommerce.MarketingModule.Core\",\"SubTotal\":100.0,\"SubTotalSecond\":0.0,\"ExcludingCategoryIds\":[],\"ExcludingProductIds\":[],\"AvailableChildren\":[],\"Children\":[],\"Id\":\"ConditionCartSubtotalLeast\"}],\"Id\":\"BlockCartCondition\"},{\"$type\":\"VirtoCommerce.MarketingModule.Data.Conditions.BlockCatalogCondition, VirtoCommerce.MarketingModule.Core\",\"All\":false,\"Not\":false,\"AvailableChildren\":[],\"Children\":[{\"$type\":\"VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions.ConditionCurrencyIs, VirtoCommerce.MarketingModule.Core\",\"Currency\":\"usd\",\"AvailableChildren\":[],\"Children\":[],\"Id\":\"ConditionCurrencyIs\"},{\"$type\":\"VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions.ConditionInStockQuantity, VirtoCommerce.MarketingModule.Core\",\"Quantity\":6,\"QuantitySecond\":0,\"CompareCondition\":\"AtLeast\",\"Exactly\":false,\"AvailableChildren\":[],\"Children\":[],\"Id\":\"ConditionInStockQuantity\"}],\"Id\":\"BlockCatalogCondition\"},{\"$type\":\"VirtoCommerce.MarketingModule.Core.Model.Promotions.RewardBlock, VirtoCommerce.MarketingModule.Core\",\"AvailableChildren\":[],\"Children\":[{\"$type\":\"VirtoCommerce.MarketingModule.Core.Model.Promotions.RewardCartGetOfRelSubtotal, VirtoCommerce.MarketingModule.Core\",\"Amount\":15.0,\"MaxLimit\":20.0,\"AvailableChildren\":[],\"Children\":[],\"Id\":\"RewardCartGetOfRelSubtotal\"},{\"$type\":\"VirtoCommerce.MarketingModule.Core.Model.Promotions.RewardItemGetOfRel, VirtoCommerce.MarketingModule.Core\",\"Amount\":25.0,\"ProductId\":null,\"ProductName\":null,\"MaxLimit\":30.0,\"AvailableChildren\":[],\"Children\":[],\"Id\":\"RewardItemGetOfRel\"}],\"Id\":\"RewardBlock\"}],\"Id\":\"PromoDynamicCondition\"}"
            } });
            var context = new PromotionEvaluationContext() { IsRegisteredUser = true, PromoEntries = new List<ProductPromoEntry> { new ProductPromoEntry() { ProductId = "1" } } };

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
