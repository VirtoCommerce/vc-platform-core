using System.Linq;
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

        PromotionEntity[] GetPromotionsByIds(string[] ids);       
        DynamicContentFolderEntity[] GetContentFoldersByIds(string[] ids);
        DynamicContentItemEntity[] GetContentItemsByIds(string[] ids);
        DynamicContentPlaceEntity[] GetContentPlacesByIds(string[] ids);
        DynamicContentPublishingGroupEntity[] GetContentPublicationsByIds(string[] ids);

        void RemoveFolders(string[] ids);
        void RemoveContentPublications(string[] ids);
        void RemovePlaces(string[] ids);
        void RemoveContentItems(string[] ids);
        void RemovePromotions(string[] ids);

        CouponEntity[] GetCouponsByIds(string[] ids);
        void RemoveCoupons(string[] ids);

        PromotionUsageEntity[] GetMarketingUsagesByIds(string[] ids);
        void RemoveMarketingUsages(string[] ids);
    }
}
