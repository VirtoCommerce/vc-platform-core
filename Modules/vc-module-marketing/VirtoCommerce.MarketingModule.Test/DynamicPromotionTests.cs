using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Promotions;
using VirtoCommerce.MarketingModule.Core.Search;
using VirtoCommerce.MarketingModule.Data.Promotions;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace VirtoCommerce.MarketingModule.Test
{
    [Trait("Category", "CI")]
    public class DynamicPromotionTests
    {
        [Theory]
        [InlineData(9, 10, 0)]
        [InlineData(10, 10, 0)]
        [InlineData(11, 10, 1)]
        public async void FindValidCoupon_UsesNumber(int maxUsesNumber, int totalUses, int expectedCouponsCount)
        {
            //Arrange
            var testCoupon = new Coupon() { Id = "1", Code = "1", MaxUsesNumber = maxUsesNumber, };

            var dynamicPromotion = CreateDynamicPromotion(totalUses, testCoupon);

            //Act
            var validCoupons = await dynamicPromotion.FindValidCouponsAsync(new List<string>() { "any coupon" }, null);

            //Assert
            Assert.Equal(expectedCouponsCount, validCoupons.Count());
        }

        [Theory]
        [InlineData(9, 10, 0, "userId")]
        [InlineData(10, 10, 0, "userId")]
        [InlineData(11, 10, 1, "userId")]
        public async void FindValidCoupon_UsesNumberWithUserId(int maxUsesNumber, int totalUses, int expectedCouponsCount,
            string userId)
        {
            //Arrange
            var testCoupon = new Coupon() { Id = "1", Code = "1", MaxUsesNumber = maxUsesNumber, };

            var dynamicPromotion = CreateDynamicPromotion(totalUses, testCoupon);

            //Act
            var validCouponsWithUserId = await dynamicPromotion.FindValidCouponsAsync(new List<string>() { "any coupon" }, userId);

            //Assert
            Assert.Equal(expectedCouponsCount, validCouponsWithUserId.Count());
        }

        public static IEnumerable<object[]> ExpirationDateData =>
            new List<object[]>
            {
                new object[] {DateTime.UtcNow.AddDays(-1), 0}, new object[] {DateTime.UtcNow.AddDays(1), 1}
            };

        [Theory]
        [MemberData(nameof(ExpirationDateData))]
        public async void FindValidCoupon_ExpirationDate(DateTime expirationDate, int expectedCouponsCount)
        {
            //Arrange
            var testCoupon = new Coupon() { Id = "1", Code = "1", ExpirationDate = expirationDate };

            var dynamicPromotion = CreateDynamicPromotion(0, testCoupon);

            //Act
            var validCoupons = await dynamicPromotion.FindValidCouponsAsync(new List<string>() { "any coupon" }, null);

            //Assert
            Assert.Equal(expectedCouponsCount, validCoupons.Count());
        }

        private DynamicPromotionMoq CreateDynamicPromotion(int totalUses, Coupon testCoupon)
        {
            var coupons = new List<Coupon>() { testCoupon };

            var promotionUsageServiceMoq = new Mock<IPromotionUsageSearchService>();
            promotionUsageServiceMoq.Setup(x => x.SearchUsagesAsync(It.IsAny<PromotionUsageSearchCriteria>()))
                .ReturnsAsync(new PromotionUsageSearchResult() { TotalCount = totalUses });

            var couponServiceMoq = new Mock<ICouponSearchService>();
            couponServiceMoq.Setup(x => x.SearchCouponsAsync(It.IsAny<CouponSearchCriteria>()))
                .ReturnsAsync(new CouponSearchResult() { Results = coupons });

            var result = new DynamicPromotionMoq()
            {
                CouponSearchService = couponServiceMoq.Object,
                PromotionUsageSearchService = promotionUsageServiceMoq.Object
            };

            return result;
        }

        private class DynamicPromotionMoq : DynamicPromotion
        {
            public new async Task<IEnumerable<Coupon>> FindValidCouponsAsync(ICollection<string> couponCodes, string userId)
            {
                return await base.FindValidCouponsAsync(couponCodes, userId);
            }
        }
    }
}
