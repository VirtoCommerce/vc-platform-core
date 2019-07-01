using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Repositories
{
    public interface IMarketingRepository : IRepository
    {
        IQueryable<PromotionEntity> Promotions { get; }
        IQueryable<CouponEntity> Coupons { get; }
        IQueryable<PromotionUsageEntity> PromotionUsages { get; }
        IQueryable<DynamicContentFolderEntity> Folders { get; }
        IQueryable<DynamicContentItemEntity> Items { get; }
        IQueryable<DynamicContentPlaceEntity> Places { get; }
        IQueryable<DynamicContentPublishingGroupEntity> PublishingGroups { get; }
        IQueryable<PublishingGroupContentItemEntity> PublishingGroupContentItems { get; }
        IQueryable<PublishingGroupContentPlaceEntity> PublishingGroupContentPlaces { get; }
        IQueryable<PromotionStoreEntity> PromotionStores { get; }

        Task<PromotionEntity[]> GetPromotionsByIdsAsync(string[] ids);
        Task<DynamicContentFolderEntity[]> GetContentFoldersByIdsAsync(string[] ids);
        Task<DynamicContentItemEntity[]> GetContentItemsByIdsAsync(string[] ids);
        Task<DynamicContentPlaceEntity[]> GetContentPlacesByIdsAsync(string[] ids);
        Task<DynamicContentPublishingGroupEntity[]> GetContentPublicationsByIdsAsync(string[] ids);

        Task RemoveFoldersAsync(string[] ids);
        Task RemoveContentPublicationsAsync(string[] ids);
        Task RemovePlacesAsync(string[] ids);
        Task RemoveContentItemsAsync(string[] ids);
        Task RemovePromotionsAsync(string[] ids);

        Task<CouponEntity[]> GetCouponsByIdsAsync(string[] ids);
        Task RemoveCouponsAsync(string[] ids);

        Task<PromotionUsageEntity[]> GetMarketingUsagesByIdsAsync(string[] ids);
        Task RemoveMarketingUsagesAsync(string[] ids);
    }
}
