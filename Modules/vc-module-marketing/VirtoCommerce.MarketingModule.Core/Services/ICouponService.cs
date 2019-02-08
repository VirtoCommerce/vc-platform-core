using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface ICouponService
    {
        GenericSearchResult<Coupon> SearchCoupons(CouponSearchCriteria criteria);
        Coupon[] GetByIds(string[] ids);
        void SaveCoupons(Coupon[] coupons);
        void DeleteCoupons(string[] ids);
    }
}
