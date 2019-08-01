using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.MarketingModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.MarketingModule.Data.Repositories
{
    public class MarketingRepository : DbContextRepositoryBase<MarketingDbContext>, IMarketingRepository
    {
        private readonly MarketingDbContext _dbContext;
        public MarketingRepository(MarketingDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        #region IMarketingRepository Members

        public IQueryable<PromotionEntity> Promotions => DbContext.Set<PromotionEntity>();

        public IQueryable<CouponEntity> Coupons => DbContext.Set<CouponEntity>();

        public IQueryable<PromotionUsageEntity> PromotionUsages => DbContext.Set<PromotionUsageEntity>();

        public IQueryable<DynamicContentFolderEntity> Folders => DbContext.Set<DynamicContentFolderEntity>();

        public IQueryable<DynamicContentItemEntity> Items => DbContext.Set<DynamicContentItemEntity>();

        public IQueryable<DynamicContentPlaceEntity> Places => DbContext.Set<DynamicContentPlaceEntity>();

        public IQueryable<DynamicContentPublishingGroupEntity> PublishingGroups => DbContext.Set<DynamicContentPublishingGroupEntity>();

        public IQueryable<PublishingGroupContentItemEntity> PublishingGroupContentItems => DbContext.Set<PublishingGroupContentItemEntity>();

        public IQueryable<PublishingGroupContentPlaceEntity> PublishingGroupContentPlaces => DbContext.Set<PublishingGroupContentPlaceEntity>();

        public IQueryable<PromotionStoreEntity> PromotionStores => DbContext.Set<PromotionStoreEntity>();

        public IQueryable<DynamicContentItemDynamicPropertyObjectValueEntity> DynamicContentItemDynamicPropertyObjectValues => DbContext.Set<DynamicContentItemDynamicPropertyObjectValueEntity>();

        public virtual async Task<PromotionEntity[]> GetPromotionsByIdsAsync(string[] ids)
        {
            var propmotions = await Promotions.Where(x => ids.Contains(x.Id)).ToArrayAsync();
            var storesTask = PromotionStores.Where(x => ids.Contains(x.PromotionId)).ToArrayAsync();
            var promotionsIdsWithCouponsTask = Coupons.Where(x => ids.Contains(x.PromotionId)).Select(x => x.PromotionId).Distinct().ToArrayAsync();
            await Task.WhenAll(storesTask, promotionsIdsWithCouponsTask);

            foreach (var promotion in propmotions)
            {
                promotion.HasCoupons = promotionsIdsWithCouponsTask.Result.Contains(promotion.Id);
            }
            return propmotions;
        }

        public async Task<DynamicContentFolderEntity[]> GetContentFoldersByIdsAsync(string[] ids)
        {
            var retVal = await Folders.Where(x => ids.Contains(x.Id)).ToArrayAsync();
            if (retVal.Any())
            {
                var allParentFoldersIds = retVal.Where(x => x.ParentFolderId != null).Select(x => x.ParentFolderId).ToArray();
                var allParentFolders = await GetContentFoldersByIdsAsync(allParentFoldersIds);
                foreach (var folder in retVal.Where(x => x.ParentFolderId != null))
                {
                    folder.ParentFolder = allParentFolders.FirstOrDefault(x => x.Id == folder.ParentFolderId);
                }
            }
            return retVal;
        }

        public async Task<DynamicContentItemEntity[]> GetContentItemsByIdsAsync(string[] ids)
        {
            var retVal = await Items.Where(x => ids.Contains(x.Id)).ToArrayAsync();
            if (retVal != null)
            {
                var allFoldersIds = retVal.Where(x => x.FolderId != null).Select(x => x.FolderId).Distinct().ToArray();
                var allFolders = await GetContentFoldersByIdsAsync(allFoldersIds);
                foreach (var item in retVal.Where(x => x.FolderId != null))
                {
                    item.Folder = allFolders.FirstOrDefault(x => x.Id == item.FolderId);
                }

                var dynamicContentItemDynamicPropertyObjectValues = await DynamicContentItemDynamicPropertyObjectValues.Where(x => ids.Contains(x.Id)).ToArrayAsync();
            }
            return retVal;
        }

        public async Task<DynamicContentPlaceEntity[]> GetContentPlacesByIdsAsync(string[] ids)
        {
            var retVal = await Places.Where(x => ids.Contains(x.Id)).ToArrayAsync();
            if (retVal != null)
            {
                var allFoldersIds = retVal.Where(x => x.FolderId != null).Select(x => x.FolderId).Distinct().ToArray();
                var allFolders = await GetContentFoldersByIdsAsync(allFoldersIds);
                foreach (var place in retVal.Where(x => x.FolderId != null))
                {
                    place.Folder = allFolders.FirstOrDefault(x => x.Id == place.FolderId);
                }
            }
            return retVal;
        }

        public Task<DynamicContentPublishingGroupEntity[]> GetContentPublicationsByIdsAsync(string[] ids)
        {
            return PublishingGroups.Where(i => ids.Contains(i.Id))
                .Include(x => x.ContentItems).ThenInclude(y => y.ContentItem)
                .Include(x => x.ContentPlaces).ThenInclude(y => y.ContentPlace)
                                    .ToArrayAsync();
        }

        public Task RemoveFoldersAsync(string[] ids)
        {
            const string queryPattern = @"DELETE FROM DynamicContentFolder WHERE Id IN (@Ids)";
            var name = new SqlParameter("@Ids", string.Join(", ", ids));
            return _dbContext.Database.ExecuteSqlCommandAsync(queryPattern, name);
        }


        public Task RemoveContentPublicationsAsync(string[] ids)
        {
            const string queryPattern = @"DELETE FROM DynamicContentPublishingGroup WHERE Id IN (@Ids)";
            var name = new SqlParameter("@Ids", string.Join(", ", ids));
            return _dbContext.Database.ExecuteSqlCommandAsync(queryPattern, name);
        }

        public Task RemovePlacesAsync(string[] ids)
        {
            const string queryPattern = @"DELETE FROM DynamicContentPlace WHERE Id IN (@Ids)";
            var name = new SqlParameter("@Ids", string.Join(", ", ids));
            return _dbContext.Database.ExecuteSqlCommandAsync(queryPattern, name);
        }

        public Task RemoveContentItemsAsync(string[] ids)
        {
            const string queryPattern = @"DELETE FROM DynamicContentItem WHERE Id IN (@Ids)";
            var name = new SqlParameter("@Ids", string.Join(", ", ids));
            return _dbContext.Database.ExecuteSqlCommandAsync(queryPattern, name);
        }

        public virtual Task RemovePromotionsAsync(string[] ids)
        {
            const string queryPattern = @"DELETE FROM Promotion WHERE Id IN (@Ids)";
            var name = new SqlParameter("@Ids", string.Join(", ", ids));
            return _dbContext.Database.ExecuteSqlCommandAsync(queryPattern, name);
        }

        public async Task<CouponEntity[]> GetCouponsByIdsAsync(string[] ids)
        {
            var retVal = await Coupons.Where(x => ids.Contains(x.Id)).ToArrayAsync();
            var couponCodes = retVal.Select(x => x.Code).ToArray();
            var couponUsagesTotals = await PromotionUsages.Where(x => couponCodes.Contains(x.CouponCode)).GroupBy(x => x.CouponCode)
                   .Select(x => new { CouponCode = x.Key, TotalUsesCount = x.Count() }).ToArrayAsync();
            foreach (var totalsUses in couponUsagesTotals)
            {
                var coupon = retVal.FirstOrDefault(x => x.Code.EqualsInvariant(totalsUses.CouponCode));
                if (coupon != null)
                {
                    coupon.TotalUsesCount = totalsUses.TotalUsesCount;
                }
            }
            return retVal;
        }

        public Task RemoveCouponsAsync(string[] ids)
        {
            const string queryPattern = @"DELETE FROM Coupon WHERE Id IN (@Ids)";
            var name = new SqlParameter("@Ids", string.Join(", ", ids));
            return _dbContext.Database.ExecuteSqlCommandAsync(queryPattern, name);
        }

        public Task<PromotionUsageEntity[]> GetMarketingUsagesByIdsAsync(string[] ids)
        {
            return PromotionUsages.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        }

        public Task RemoveMarketingUsagesAsync(string[] ids)
        {
            const string queryPattern = @"DELETE FROM PromotionUsage WHERE Id IN (@Ids)";
            var name = new SqlParameter("@Ids", string.Join(", ", ids));
            return _dbContext.Database.ExecuteSqlCommandAsync(queryPattern, name);
        }
        #endregion
    }

}
