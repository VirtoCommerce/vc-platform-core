using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Conditions.CartConditions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Conditions.CatalogConditions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Rewards;

namespace VirtoCommerce.PricingModule.Test.DynamicExpressions
{
    public class EvaluationTestDataGenerator
    {
        public static IEnumerable<object[]> GetConditions()
        {
            var inputDataCollection = new[]
            {
                ConditionEntryIsInputData(),
                ConditionCurrencyIsInputData(),
                ConditionCodeContainsInputData(),
                ConditionCategoryIsInputData(),
                ConditionInStockQuantityInputData()
            };

            foreach (var data in inputDataCollection.SelectMany(d => d))
                yield return data;
        }

        #region ConditionCurrencyIs
        public static IEnumerable<object[]> ConditionCurrencyIsInputData()
        {
            string currency1 = "USD";
            string currency2 = "EUR";

            IEvaluationContext context = new PromotionEvaluationContext
            {
                Currency = currency1,
                PromoEntries = new List<ProductPromoEntry>
                {
                    new ProductPromoEntry(),
                    new ProductPromoEntry(),
                    new ProductPromoEntry()
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionCurrencyIs { Currency = currency1 } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 3,
                    InvalidCount = 0
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionCurrencyIs { Currency = currency2 } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 0,
                    InvalidCount = 3
                }
            };
        }
        #endregion

        #region ConditionEntryIs
        private static IEnumerable<object[]> ConditionEntryIsInputData()
        {
            string productId = Guid.NewGuid().ToString();

            IEvaluationContext context = new PromotionEvaluationContext
            {
                PromoEntries = new List<ProductPromoEntry>
                {
                    new ProductPromoEntry { ProductId = productId },
                    new ProductPromoEntry { ProductId = Guid.NewGuid().ToString() },
                    new ProductPromoEntry { ProductId = Guid.NewGuid().ToString() }
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionEntryIs { ProductId = productId } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 1,
                    InvalidCount = 2
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionEntryIs { ProductId = Guid.NewGuid().ToString() } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 0,
                    InvalidCount = 3
                }
            };
        }
        #endregion

        #region ConditionCodeContains
        private static IEnumerable<object[]> ConditionCodeContainsInputData()
        {
            string productCode = Guid.NewGuid().ToString();

            IEvaluationContext context = new PromotionEvaluationContext
            {
                PromoEntries = new List<ProductPromoEntry>
                {
                    new ProductPromoEntry { Code = productCode },
                    new ProductPromoEntry { Code = Guid.NewGuid().ToString() },
                    new ProductPromoEntry { Code = Guid.NewGuid().ToString() }
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionCodeContains { Keyword = productCode } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 1,
                    InvalidCount = 2
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionCodeContains { Keyword = productCode } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 1,
                    InvalidCount = 2
                }
            };
        }
        #endregion

        #region ConditionCategoryIs
        public static IEnumerable<object[]> ConditionCategoryIsInputData()
        {
            string categoryId = Guid.NewGuid().ToString();
            string productId = Guid.NewGuid().ToString();

            IEvaluationContext context = new PromotionEvaluationContext
            {
                PromoEntries = new List<ProductPromoEntry>
                {
                    new ProductPromoEntry { CategoryId = categoryId, ProductId = Guid.NewGuid().ToString() },
                    new ProductPromoEntry { CategoryId = categoryId, ProductId = productId },
                    new ProductPromoEntry { CategoryId = Guid.NewGuid().ToString(), ProductId = Guid.NewGuid().ToString() }
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionCategoryIs { CategoryId = categoryId, ExcludingProductIds = new[] { productId } } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 1,
                    InvalidCount = 2
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionCategoryIs { CategoryId = categoryId } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 2,
                    InvalidCount = 1
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionCategoryIs { CategoryId = Guid.NewGuid().ToString() } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 0,
                    InvalidCount = 3
                }
            };
        }
        #endregion

        #region ConditionInStockQuantity
        public static IEnumerable<object[]> ConditionInStockQuantityInputData()
        {
            IEvaluationContext context = new PromotionEvaluationContext
            {
                PromoEntries = new List<ProductPromoEntry>
                {
                    new ProductPromoEntry { InStockQuantity = 12 },
                    new ProductPromoEntry { InStockQuantity = 10 },
                    new ProductPromoEntry { InStockQuantity = 8 }
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionInStockQuantity { Quantity = 7, CompareCondition = "AtLeast" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 3,
                    InvalidCount = 0
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionInStockQuantity { Quantity = 10, CompareCondition = "IsLessThanOrEqual" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 2,
                    InvalidCount = 1
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionInStockQuantity { Quantity = 10, CompareCondition = "Exactly" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 1,
                    InvalidCount = 2
                }
            };

            yield return new object[]
            {
                new IConditionExpression[]
                {
                    new ConditionInStockQuantity { Quantity = 8, QuantitySecond = 10, CompareCondition = "Between" },
                },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 2,
                    InvalidCount = 1
                }
            };
        }
        #endregion

        #region ConditionAtCartItemExtendedTotal
        public static IEnumerable<object[]> ConditionAtCartItemExtendedTotalInputData()
        {
            IEvaluationContext context = new PromotionEvaluationContext
            {
                PromoEntries = new List<ProductPromoEntry>
                {
                    new ProductPromoEntry { Price = 5, Quantity = 3 },
                    new ProductPromoEntry { Price = 10, Quantity = 2 },
                    new ProductPromoEntry { Price = 30, Quantity = 1 },
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionAtCartItemExtendedTotal { LineItemTotal = 20, CompareCondition = "AtLeast" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 2,
                    InvalidCount = 1
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionAtCartItemExtendedTotal { LineItemTotal = 15, CompareCondition = "Exactly" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 1,
                    InvalidCount = 2
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionAtCartItemExtendedTotal { LineItemTotal = 20, LineItemTotalSecond = 30, CompareCondition = "Between" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 2,
                    InvalidCount = 1
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionAtCartItemExtendedTotal { LineItemTotal = 30, CompareCondition = "IsLessThanOrEqual" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 3,
                    InvalidCount = 0
                }
            };
        }
        #endregion

        #region ConditionAtNumItemsInCart
        public static IEnumerable<object[]> ConditionAtNumItemsInCartInputData()
        {
            IEvaluationContext context = new PromotionEvaluationContext
            {
                PromoEntries = new List<ProductPromoEntry>
                {
                    new ProductPromoEntry { Quantity = 2 },
                    new ProductPromoEntry { Quantity = 3 },
                    new ProductPromoEntry { Quantity = 5 },
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionAtNumItemsInCart { NumItem = 11, CompareCondition = "AtLeast" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 0,
                    InvalidCount = 3
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionAtNumItemsInCart { NumItem = 10, CompareCondition = "Exactly" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 3,
                    InvalidCount = 0
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionAtNumItemsInCart { NumItem = 0, NumItemSecond = 10, CompareCondition = "Between" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 3,
                    InvalidCount = 0
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionAtNumItemsInCart { NumItem = 9, CompareCondition = "IsLessThanOrEqual" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 0,
                    InvalidCount = 3
                }
            };
        }
        #endregion

        #region ConditionCartSubtotalLeast
        public static IEnumerable<object[]> ConditionCartSubtotalLeastInputData()
        {
            IEvaluationContext context = new PromotionEvaluationContext
            {
                PromoEntries = new List<ProductPromoEntry>
                {
                    new ProductPromoEntry { Quantity = 2, Price = 100 },
                    new ProductPromoEntry { Quantity = 3, Price = 50 },
                    new ProductPromoEntry { Quantity = 15, Price = 10 },
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionCartSubtotalLeast { SubTotal = 600, CompareCondition = "AtLeast" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 0,
                    InvalidCount = 3
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionAtNumItemsInCart { NumItem = 500, CompareCondition = "Exactly" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 3,
                    InvalidCount = 0
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionAtNumItemsInCart { NumItem = 450, NumItemSecond = 550, CompareCondition = "Between" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 3,
                    InvalidCount = 0
                }
            };

            yield return new object[]
            {
                new IConditionExpression[] { new ConditionAtNumItemsInCart { NumItem = 450, CompareCondition = "IsLessThanOrEqual" } },
                new IRewardExpression[] { new RewardItemGetOfRel() },
                context,
                new DynamicPromotionEvaluationResult
                {
                    ValidCount = 0,
                    InvalidCount = 3
                }
            };
        }
        #endregion
    }
}
