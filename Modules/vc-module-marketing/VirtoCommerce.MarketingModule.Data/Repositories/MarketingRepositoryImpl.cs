using System;
using System.Linq;
using VirtoCommerce.MarketingModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Repositories
{
    public class MarketingRepositoryImpl : DbContextRepositoryBase, IMarketingRepository
    {
        public MarketingRepositoryImpl()
        {
        }

        public MarketingRepositoryImpl(string nameOrConnectionString, params IInterceptor[] interceptors)
            : base(nameOrConnectionString, null, interceptors)
        {
            Configuration.LazyLoadingEnabled = false;

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CouponEntity>().ToTable("Coupon");
            modelBuilder.Entity<CouponEntity>().HasKey(x => x.Id)
                        .Property(x => x.Id);
            modelBuilder.Entity<CouponEntity>().HasRequired(x => x.Promotion)
                                .WithMany()
                                .HasForeignKey(x => x.PromotionId).WillCascadeOnDelete(true);


            modelBuilder.Entity<PromotionEntity>().ToTable("Promotion");
            modelBuilder.Entity<PromotionEntity>().HasKey(x => x.Id)
                        .Property(x => x.Id);

            modelBuilder.Entity<PromotionUsageEntity>().ToTable("PromotionUsage");
            modelBuilder.Entity<PromotionUsageEntity>().HasKey(x => x.Id)
                        .Property(x => x.Id);
            modelBuilder.Entity<PromotionUsageEntity>().HasRequired(x => x.Promotion)
                                   .WithMany()
                                   .HasForeignKey(x => x.PromotionId).WillCascadeOnDelete(true);

            modelBuilder.Entity<DynamicContentItemEntity>().ToTable("DynamicContentItem");
            modelBuilder.Entity<DynamicContentItemEntity>().HasKey(x => x.Id)
                        .Property(x => x.Id);
            modelBuilder.Entity<DynamicContentItemEntity>().HasOptional(x => x.Folder)
                                   .WithMany(x => x.ContentItems)
                                   .HasForeignKey(x => x.FolderId).WillCascadeOnDelete(true);

            modelBuilder.Entity<DynamicContentPlaceEntity>().ToTable("DynamicContentPlace");
            modelBuilder.Entity<DynamicContentPlaceEntity>().HasKey(x => x.Id)
                    .Property(x => x.Id);
            modelBuilder.Entity<DynamicContentPlaceEntity>().HasOptional(x => x.Folder)
                               .WithMany(x => x.ContentPlaces)
                               .HasForeignKey(x => x.FolderId).WillCascadeOnDelete(true);

            modelBuilder.Entity<DynamicContentPublishingGroupEntity>().ToTable("DynamicContentPublishingGroup");
            modelBuilder.Entity<DynamicContentPublishingGroupEntity>().HasKey(x => x.Id)
                    .Property(x => x.Id);

            modelBuilder.Entity<PublishingGroupContentItemEntity>().ToTable("PublishingGroupContentItem");
            modelBuilder.Entity<PublishingGroupContentItemEntity>().HasKey(x => x.Id)
                    .Property(x => x.Id);
            modelBuilder.Entity<PublishingGroupContentItemEntity>().HasRequired(p => p.ContentItem)
                    .WithMany().HasForeignKey(x => x.DynamicContentItemId)
                    .WillCascadeOnDelete(true);
            modelBuilder.Entity<PublishingGroupContentItemEntity>().HasRequired(p => p.PublishingGroup)
                  .WithMany(x => x.ContentItems).HasForeignKey(x => x.DynamicContentPublishingGroupId)
                  .WillCascadeOnDelete(true);

            modelBuilder.Entity<PublishingGroupContentPlaceEntity>().ToTable("PublishingGroupContentPlace");
            modelBuilder.Entity<PublishingGroupContentPlaceEntity>().HasKey(x => x.Id)
                    .Property(x => x.Id);
            modelBuilder.Entity<PublishingGroupContentPlaceEntity>().HasRequired(p => p.ContentPlace).WithMany()
                .HasForeignKey(x => x.DynamicContentPlaceId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<PublishingGroupContentPlaceEntity>().HasRequired(p => p.PublishingGroup)
               .WithMany(x => x.ContentPlaces).HasForeignKey(x => x.DynamicContentPublishingGroupId)
               .WillCascadeOnDelete(true);

            modelBuilder.Entity<DynamicContentFolderEntity>().ToTable("DynamicContentFolder");
            modelBuilder.Entity<DynamicContentFolderEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);

            modelBuilder.Entity<PromotionStoreEntity>().ToTable("PromotionStore");
            modelBuilder.Entity<PromotionStoreEntity>().HasKey(x => x.Id).Property(x => x.Id);
            modelBuilder.Entity<PromotionStoreEntity>().HasRequired(x => x.Promotion)
                .WithMany(x => x.Stores).HasForeignKey(x => x.PromotionId)
                .WillCascadeOnDelete(true);


            base.OnModelCreating(modelBuilder);
        }

        #region IMarketingRepository Members

        public IQueryable<PromotionEntity> Promotions
        {
            get { return GetAsQueryable<PromotionEntity>(); }
        }
        public IQueryable<CouponEntity> Coupons
        {
            get { return GetAsQueryable<CouponEntity>(); }
        }
        public IQueryable<PromotionUsageEntity> PromotionUsages
        {
            get { return GetAsQueryable<PromotionUsageEntity>(); }
        }

        public IQueryable<DynamicContentFolderEntity> Folders
        {
            get { return GetAsQueryable<DynamicContentFolderEntity>(); }

        }
        public IQueryable<DynamicContentItemEntity> Items
        {
            get { return GetAsQueryable<DynamicContentItemEntity>(); }
        }

        public IQueryable<DynamicContentPlaceEntity> Places
        {
            get { return GetAsQueryable<DynamicContentPlaceEntity>(); }
        }

        public IQueryable<DynamicContentPublishingGroupEntity> PublishingGroups
        {
            get { return GetAsQueryable<DynamicContentPublishingGroupEntity>(); }
        }

        public IQueryable<PublishingGroupContentItemEntity> PublishingGroupContentItems
        {
            get { return GetAsQueryable<PublishingGroupContentItemEntity>(); }
        }

        public IQueryable<PublishingGroupContentPlaceEntity> PublishingGroupContentPlaces
        {
            get { return GetAsQueryable<PublishingGroupContentPlaceEntity>(); }
        }

        public IQueryable<PromotionStoreEntity> PromotionStores
        {
            get { return GetAsQueryable<PromotionStoreEntity>(); }
        }

        public virtual PromotionEntity[] GetPromotionsByIds(string[] ids)
        {
            var retVal = Promotions.Where(x => ids.Contains(x.Id)).ToArray();
            var stores = PromotionStores.Where(x => ids.Contains(x.PromotionId)).ToArray();
            var promotionsIdsWithCoupons = Coupons.Where(x => ids.Contains(x.PromotionId)).Select(x => x.PromotionId).Distinct().ToArray();
            foreach (var promotion in retVal)
            {
                promotion.HasCoupons = promotionsIdsWithCoupons.Contains(promotion.Id);
            }
            return retVal;
        }

        public DynamicContentFolderEntity[] GetContentFoldersByIds(string[] ids)
        {
            var retVal = Folders.Where(x => ids.Contains(x.Id)).ToArray();
            if (retVal.Any())
            {
                var allParentFoldersIds = retVal.Where(x => x.ParentFolderId != null).Select(x => x.ParentFolderId).ToArray();
                var allParentFolders = GetContentFoldersByIds(allParentFoldersIds);
                foreach (var folder in retVal.Where(x => x.ParentFolderId != null))
                {
                    folder.ParentFolder = allParentFolders.FirstOrDefault(x => x.Id == folder.ParentFolderId);
                }
            }
            return retVal;
        }

        public DynamicContentItemEntity[] GetContentItemsByIds(string[] ids)
        {
            var retVal = Items.Where(x => ids.Contains(x.Id)).ToArray();
            if (retVal != null)
            {
                var allFoldersIds = retVal.Where(x => x.FolderId != null).Select(x => x.FolderId).Distinct().ToArray();
                var allFolders = GetContentFoldersByIds(allFoldersIds);
                foreach (var item in retVal.Where(x => x.FolderId != null))
                {
                    item.Folder = allFolders.FirstOrDefault(x => x.Id == item.FolderId);
                }
            }
            return retVal;
        }

        public DynamicContentPlaceEntity[] GetContentPlacesByIds(string[] ids)
        {
            var retVal = Places.Where(x => ids.Contains(x.Id)).ToArray();
            if (retVal != null)
            {
                var allFoldersIds = retVal.Where(x => x.FolderId != null).Select(x => x.FolderId).Distinct().ToArray();
                var allFolders = GetContentFoldersByIds(allFoldersIds);
                foreach (var place in retVal.Where(x => x.FolderId != null))
                {
                    place.Folder = allFolders.FirstOrDefault(x => x.Id == place.FolderId);
                }
            }
            return retVal;
        }

        public DynamicContentPublishingGroupEntity[] GetContentPublicationsByIds(string[] ids)
        {
            return PublishingGroups.Include(x => x.ContentItems.Select(y => y.ContentItem))
                                    .Include(x => x.ContentPlaces.Select(y => y.ContentPlace))
                                    .Where(x => ids.Contains(x.Id)).ToArray();
        }

        public void RemoveFolders(string[] ids)
        {
            const string queryPattern = @"DELETE FROM DynamicContentFolder WHERE Id IN ({0})";
            var query = string.Format(queryPattern, string.Join(", ", ids.Select(x => string.Format("'{0}'", x))));
            ObjectContext.ExecuteStoreCommand(query);
        }


        public void RemoveContentPublications(string[] ids)
        {
            const string queryPattern = @"DELETE FROM DynamicContentPublishingGroup WHERE Id IN ({0})";
            var query = string.Format(queryPattern, string.Join(", ", ids.Select(x => string.Format("'{0}'", x))));
            ObjectContext.ExecuteStoreCommand(query);
        }

        public void RemovePlaces(string[] ids)
        {
            const string queryPattern = @"DELETE FROM DynamicContentPlace WHERE Id IN ({0})";
            var query = string.Format(queryPattern, string.Join(", ", ids.Select(x => string.Format("'{0}'", x))));
            ObjectContext.ExecuteStoreCommand(query);
        }

        public void RemoveContentItems(string[] ids)
        {
            const string queryPattern = @"DELETE FROM DynamicContentItem WHERE Id IN ({0})";
            var query = string.Format(queryPattern, string.Join(", ", ids.Select(x => string.Format("'{0}'", x))));
            ObjectContext.ExecuteStoreCommand(query);
        }

        public virtual void RemovePromotions(string[] ids)
        {
            const string queryPattern = @"DELETE FROM Promotion WHERE Id IN ({0})";
            var query = string.Format(queryPattern, string.Join(", ", ids.Select(x => string.Format("'{0}'", x))));
            ObjectContext.ExecuteStoreCommand(query);
        }

        public CouponEntity[] GetCouponsByIds(string[] ids)
        {
            var retVal = Coupons.Where(x => ids.Contains(x.Id)).ToArray();
            var couponCodes = retVal.Select(x => x.Code).ToArray();
            var couponUsagesTotals = PromotionUsages.Where(x => couponCodes.Contains(x.CouponCode)).GroupBy(x => x.CouponCode)
                   .Select(x => new { CouponCode = x.Key, TotalUsesCount = x.Count() }).ToArray();
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

        public void RemoveCoupons(string[] ids)
        {
            const string queryPattern = @"DELETE FROM Coupon WHERE Id IN ({0})";
            var query = string.Format(queryPattern, string.Join(", ", ids.Select(x => string.Format("'{0}'", x))));
            ObjectContext.ExecuteStoreCommand(query);
        }

        public PromotionUsageEntity[] GetMarketingUsagesByIds(string[] ids)
        {
            var retVal = PromotionUsages.Where(x => ids.Contains(x.Id)).ToArray();
            return retVal;
        }

        public void RemoveMarketingUsages(string[] ids)
        {
            const string queryPattern = @"DELETE FROM PromotionUsage WHERE Id IN ({0})";
            var query = string.Format(queryPattern, string.Join(", ", ids.Select(x => string.Format("'{0}'", x))));
            ObjectContext.ExecuteStoreCommand(query);
        }
        #endregion
    }

}
