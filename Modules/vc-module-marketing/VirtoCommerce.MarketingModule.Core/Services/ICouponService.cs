using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface ICouponService
    {
        Task<GenericSearchResult<Coupon>> SearchCouponsAsync(CouponSearchCriteria criteria);
        Task<Coupon[]> GetByIdsAsync(string[] ids);
        Task SaveCouponsAsync(Coupon[] coupons);
        Task DeleteCouponsAsync(string[] ids);
    }
}
