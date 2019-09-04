using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Promotions;
using VirtoCommerce.MarketingModule.Core.Search;
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
        public void EvaluateRewards_OnlySingleExclusivePromotion()
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
            var couponSearchMockServiceMock = new Mock<ICouponSearchService>();
            var promotionUsageSearchMock = new Mock<IPromotionUsageSearchService>();
            var promoConditionTree = "{\"AvailableChildren\":null,\"Children\":[{\"All\":false,\"Not\":false,\"AvailableChildren\":null,\"Children\":[{\"AvailableChildren\":null,\"Children\":[],\"Id\":\"ConditionIsRegisteredUser\"}],\"Id\":\"BlockCustomerCondition\"},{\"All\":false,\"Not\":false,\"AvailableChildren\":null,\"Children\":[],\"Id\":\"BlockCatalogCondition\"},{\"All\":false,\"Not\":false,\"AvailableChildren\":null,\"Children\":[{\"NumItem\":10,\"NumItemSecond\":13,\"ProductId\":null,\"ProductName\":null,\"CompareCondition\":\"Between\",\"AvailableChildren\":null,\"Children\":[],\"Id\":\"ConditionAtNumItemsInCart\"},{\"SubTotal\":0.0,\"SubTotalSecond\":100.0,\"ExcludingCategoryIds\":[],\"ExcludingProductIds\":[],\"CompareCondition\":\"AtLeast\",\"AvailableChildren\":null,\"Children\":[],\"Id\":\"ConditionCartSubtotalLeast\"}],\"Id\":\"BlockCartCondition\"},{\"AvailableChildren\":null,\"Children\":[{\"Amount\":15.0,\"AvailableChildren\":null,\"Children\":[],\"Id\":\"RewardCartGetOfAbsSubtotal\"}],\"Id\":\"BlockReward\"}],\"Id\":\"PromotionConditionAndRewardTree\"}";

            AbstractTypeFactory<Promotion>.RegisterType<DynamicPromotion>().WithSetupAction((promotion) =>
            {
                var dynamicPromotion = promotion as DynamicPromotion;
                dynamicPromotion.CouponSearchService = couponSearchMockServiceMock.Object;
                dynamicPromotion.PromotionUsageSearchService = promotionUsageSearchMock.Object;
                dynamicPromotion.DynamicExpression = AbstractTypeFactory<PromotionConditionAndRewardTree>.TryCreateInstance();
                dynamicPromotion.DynamicExpression.Children = dynamicPromotion.DynamicExpression.AvailableChildren.ToList();
            });

            foreach (var conditionTree in AbstractTypeFactory<PromotionConditionAndRewardTreePrototype>.TryCreateInstance().Traverse<IConditionTree>(x => x.AvailableChildren))
            {
                AbstractTypeFactory<IConditionTree>.RegisterType(conditionTree.GetType());
            }

            var evalPolicy = GetPromotionEvaluationPolicy(new List<Promotion> { new DynamicPromotion
            {
                CouponSearchService = couponSearchMockServiceMock.Object,
                PromotionUsageSearchService = promotionUsageSearchMock.Object,
                DynamicExpression = JsonConvert.DeserializeObject<PromotionConditionAndRewardTree>(promoConditionTree, new ConditionJsonConverter(), new RewardJsonConverter())
            } });

            var context = new PromotionEvaluationContext()
            {
                IsRegisteredUser = true,
                IsEveryone = true,
                Currency = "usd",
                PromoEntries = new List<ProductPromoEntry> { new ProductPromoEntry() { ProductId = "1" } },
                CartPromoEntries = new List<ProductPromoEntry> { new ProductPromoEntry { Quantity = 11, Price = 5 } }
            };

            

            //RegisterConditionRewards();

            //Act
            var rewards = evalPolicy.EvaluatePromotionAsync(context).GetAwaiter().GetResult().Rewards;

            //Assert
            Assert.Equal(1, rewards.Count);
        }

        private static IMarketingPromoEvaluator GetPromotionEvaluationPolicy(IEnumerable<Promotion> promotions)
        {
            var result = new PromotionSearchResult
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
            AbstractTypeFactory<IConditionTree>.RegisterType<PromotionConditionAndRewardTree>();
            AbstractTypeFactory<IConditionTree>.RegisterType<BlockConditionAndOr>();

            AbstractTypeFactory<IConditionTree>.RegisterType<BlockCustomerCondition>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionIsRegisteredUser>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionIsEveryone>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionIsFirstTimeBuyer>();
            AbstractTypeFactory<IConditionTree>.RegisterType<UserGroupsContainsCondition>();

            AbstractTypeFactory<IConditionTree>.RegisterType<BlockCatalogCondition>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionAtNumItemsInCart>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionAtNumItemsInCategoryAreInCart>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionAtNumItemsOfEntryAreInCart>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionCartSubtotalLeast>();

            AbstractTypeFactory<IConditionTree>.RegisterType<BlockCartCondition>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionCategoryIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionCodeContains>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionCurrencyIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionEntryIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionInStockQuantity>();

            AbstractTypeFactory<IConditionTree>.RegisterType<BlockReward>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardCartGetOfAbsSubtotal>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardCartGetOfRelSubtotal>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardItemGetFreeNumItemOfProduct>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardItemGetOfAbs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardItemGetOfAbsForNum>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardItemGetOfRel>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardItemGetOfRelForNum>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardItemGiftNumItem>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardShippingGetOfAbsShippingMethod>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardShippingGetOfRelShippingMethod>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardPaymentGetOfAbs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardPaymentGetOfRel>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardItemForEveryNumInGetOfRel>();
            AbstractTypeFactory<IConditionTree>.RegisterType<RewardItemForEveryNumOtherItemInGetOfRel>();

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

        public override Task<PromotionReward[]> EvaluatePromotionAsync(IEvaluationContext context)
        {
            foreach (var reward in Rewards)
            {
                reward.Promotion = this;
            }
            return Task.FromResult(Rewards.ToArray());
        }
    }
}
