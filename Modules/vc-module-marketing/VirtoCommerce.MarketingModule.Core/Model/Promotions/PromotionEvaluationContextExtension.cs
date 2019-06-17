using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public static class PromotionEvaluationContextExtension
    {
        #region Dynamic expression evaluation helper methods

        public static int GetCartItemsQuantity(this PromotionEvaluationContext context, string[] excludingCategoryIds, string[] excludingProductIds)
        {
            var retVal = context.CartPromoEntries.ExcludeCategories(excludingCategoryIds)
                                  .ExcludeProducts(excludingProductIds)
                                  .Sum(x => x.Quantity);
            return retVal;
        }

        public static int GetCartItemsOfCategoryQuantity(this PromotionEvaluationContext context, string categoryId, string[] excludingCategoryIds, string[] excludingProductIds)
        {
            var retVal = context.CartPromoEntries.InCategories(new[] { categoryId })
                                  .ExcludeCategories(excludingCategoryIds)
                                  .ExcludeProducts(excludingProductIds)
                                  .Sum(x => x.Quantity);
            return retVal;
        }

        public static int GetCartItemsOfProductQuantity(this PromotionEvaluationContext context, string productId)
        {
            var retVal = context.CartPromoEntries.InProducts(new[] { productId })
                                  .Sum(x => x.Quantity);
            return retVal;
        }

        public static decimal GetCartTotalWithExcludings(this PromotionEvaluationContext context, string[] excludingCategoryIds, string[] excludingProductIds)
        {
            var retVal = context.CartPromoEntries.ExcludeCategories(excludingCategoryIds)
                                  .ExcludeProducts(excludingProductIds)
                                  .Sum(x => x.Price * x.Quantity);

            return retVal;
        }

        public static bool IsItemInCategory(this PromotionEvaluationContext context, string categoryId, string[] excludingCategoryIds, string[] excludingProductIds)
        {
            var result = new ProductPromoEntry[] { context.PromoEntry }.InCategories(new[] { categoryId })
                               .ExcludeCategories(excludingCategoryIds)
                               .ExcludeProducts(excludingProductIds)
                               .Any();
            return result;
        }

        public static bool IsItemCodeContains(this PromotionEvaluationContext context, string code)
        {
            var result = context.PromoEntry != null && !string.IsNullOrEmpty(context.PromoEntry.Code);
            if (result)
            {
                result = context.PromoEntry.Code.IndexOf(code, StringComparison.OrdinalIgnoreCase) != -1;
            }
            return result;
        }

        [Obsolete("Use new method instead.")]
        public static bool IsAnyLineItemExtendedTotal(this PromotionEvaluationContext context, decimal lineItemTotal, bool isExactly, string[] excludingCategoryIds, string[] excludingProductIds)
        {
            var compareCondition = ConditionOperation.AtLeast;
            if (isExactly)
            {
                compareCondition = ConditionOperation.Exactly;
            }

            return IsAnyLineItemExtendedTotalNew(context, lineItemTotal, 0, compareCondition, excludingCategoryIds, excludingProductIds);
        }

        public static bool IsAnyLineItemExtendedTotalNew(this PromotionEvaluationContext context, decimal lineItemTotal, decimal lineItemTotalSecond, string compareCondition, string[] excludingCategoryIds, string[] excludingProductIds)
        {
            if (compareCondition.EqualsInvariant(ConditionOperation.Exactly))
                return context.CartPromoEntries.Where(x => x.Price * x.Quantity == lineItemTotal)
                    .ExcludeCategories(excludingCategoryIds)
                    .ExcludeProducts(excludingProductIds)
                    .Any();
            else if (compareCondition.EqualsInvariant(ConditionOperation.AtLeast))
                return context.CartPromoEntries.Where(x => x.Price * x.Quantity >= lineItemTotal)
                    .ExcludeCategories(excludingCategoryIds)
                    .ExcludeProducts(excludingProductIds)
                    .Any();
            else if (compareCondition.EqualsInvariant(ConditionOperation.IsLessThanOrEqual))
                return context.CartPromoEntries.Where(x => x.Price * x.Quantity <= lineItemTotal)
                    .ExcludeCategories(excludingCategoryIds)
                    .ExcludeProducts(excludingProductIds)
                    .Any();
            else if (compareCondition.EqualsInvariant(ConditionOperation.Between))
                return context.CartPromoEntries.Where(x => x.Price * x.Quantity >= lineItemTotal && x.Quantity <= lineItemTotalSecond)
                    .ExcludeCategories(excludingCategoryIds)
                    .ExcludeProducts(excludingProductIds)
                    .Any();
            throw new Exception("CompareCondition has incorrect value.");
        }

        public static bool IsItemInProduct(this PromotionEvaluationContext context, string productId)
        {
            return new ProductPromoEntry[] { context.PromoEntry }.InProducts(new[] { productId }).Any();
        }

        public static bool IsItemInProducts(this PromotionEvaluationContext context, string[] productIds)
        {
            return new ProductPromoEntry[] { context.PromoEntry }.InProducts(productIds).Any();
        }

        [Obsolete("Use new method instead.")]
        public static bool IsItemsInStockQuantity(this PromotionEvaluationContext context, bool isExactly, int quantity)
        {
            var compareCondition = isExactly ? ConditionOperation.Exactly : ConditionOperation.AtLeast;
            return IsItemsInStockQuantityNew(context, compareCondition, quantity, 0);
        }

        public static bool IsItemsInStockQuantityNew(this PromotionEvaluationContext context, string compareCondition, int quantity, int quantitySecond)
        {
            if (compareCondition.EqualsInvariant(ConditionOperation.Exactly))
                return context.PromoEntry.InStockQuantity == quantity;
            else if (compareCondition.EqualsInvariant(ConditionOperation.AtLeast))
                return context.PromoEntry.InStockQuantity >= quantity;
            else if (compareCondition.EqualsInvariant(ConditionOperation.IsLessThanOrEqual))
                return context.PromoEntry.InStockQuantity <= quantity;
            else if (compareCondition.EqualsInvariant(ConditionOperation.Between))
                return context.PromoEntry.InStockQuantity >= quantity && context.PromoEntry.InStockQuantity <= quantitySecond;
            throw new Exception("CompareCondition has incorrect value.");
        }

        #endregion

        #region ProductPromoEntry extensions

        public static IEnumerable<ProductPromoEntry> InCategory(this IEnumerable<ProductPromoEntry> entries, string categoryId)
        {
            var retVal = entries.InCategories(new[] { categoryId });
            return retVal;
        }

        public static IEnumerable<ProductPromoEntry> InCategories(this IEnumerable<ProductPromoEntry> entries, string[] categoryIds)
        {
            categoryIds = categoryIds.Where(x => x != null).ToArray();
            var promotionEntries = entries as ProductPromoEntry[] ?? entries.ToArray();
            return categoryIds.Any() ? promotionEntries.Where(x => ProductInCategories(x, categoryIds)) : promotionEntries;
        }

        public static IEnumerable<ProductPromoEntry> InProduct(this IEnumerable<ProductPromoEntry> entries, string productId)
        {
            var retVal = entries.InProducts(new[] { productId });
            return retVal;
        }

        public static IEnumerable<ProductPromoEntry> InProducts(this IEnumerable<ProductPromoEntry> entries, string[] productIds)
        {
            productIds = productIds.Where(x => x != null).ToArray();
            var promotionEntries = entries as IList<ProductPromoEntry> ?? entries.ToList();
            return productIds.Any() ? promotionEntries.Where(x => ProductInProducts(x, productIds)) : promotionEntries;
        }


        public static IEnumerable<ProductPromoEntry> ExcludeCategories(this IEnumerable<ProductPromoEntry> entries, string[] categoryIds)
        {
            var retVal = entries.Where(x => !ProductInCategories(x, categoryIds));
            return retVal;
        }

        public static IEnumerable<ProductPromoEntry> ExcludeProducts(this IEnumerable<ProductPromoEntry> entries, string[] productIds)
        {
            var retval = entries.Where(x => !ProductInProducts(x, productIds));
            return retval;
        }

        public static bool ProductInCategories(this ProductPromoEntry entry, ICollection<string> categoryIds)
        {
            var result = categoryIds.Contains(entry.CategoryId, StringComparer.OrdinalIgnoreCase);
            if (!result && entry.Outline != null)
            {
                result = entry.Outline.Split(';', '/', '\\').Intersect(categoryIds, StringComparer.OrdinalIgnoreCase).Any();
            }
            return result;
        }

        public static bool ProductInProducts(this ProductPromoEntry entry, IEnumerable<string> productIds)
        {
            return productIds.Contains(entry.ProductId, StringComparer.OrdinalIgnoreCase);
        }

        #endregion
    }
}
