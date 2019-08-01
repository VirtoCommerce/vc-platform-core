using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface ICouponService
    {
        Task<Coupon[]> GetByIdsAsync(string[] ids);
        Task SaveCouponsAsync(Coupon[] coupons);
        Task DeleteCouponsAsync(string[] ids);
    }
}
