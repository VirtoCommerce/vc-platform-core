using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.Platform.Data.Repositories
{
    public class PlatformDbContext : DbContextWithTriggers
    {
        public PlatformDbContext(DbContextOptions<PlatformDbContext> options)
            :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Change logging
            modelBuilder.Entity<OperationLogEntity>().ToTable("PlatformOperationLog");
            modelBuilder.Entity<OperationLogEntity>().HasIndex(x => new { x.ObjectType, x.ObjectId })
                        .IsUnique(false)
                        .HasName("IX_ObjectType_ObjectId");
            #endregion

            #region Settings
            modelBuilder.Entity<SettingEntity>().ToTable("PlatformSetting");
            modelBuilder.Entity<SettingEntity>().HasIndex(x => new { x.ObjectType, x.ObjectId })
                        .IsUnique(false)
                        .HasName("IX_ObjectType_ObjectId");

            modelBuilder.Entity<SettingValueEntity>().ToTable("PlatformSettingValue");

            modelBuilder.Entity<SettingValueEntity>().HasOne(x => x.Setting)
                        .WithMany(x => x.SettingValues)
                        .HasForeignKey(x => x.SettingId)
                        .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Dynamic Properties

            modelBuilder.Entity<DynamicPropertyEntity>().ToTable("PlatformDynamicProperty");
            modelBuilder.Entity<DynamicPropertyEntity>().HasIndex(x => new { x.ObjectType, x.Name })
                        .HasName("IX_PlatformDynamicProperty_ObjectType_Name")
                        .IsUnique(true);               

            modelBuilder.Entity<DynamicPropertyNameEntity>().ToTable("PlatformDynamicPropertyName");
            modelBuilder.Entity<DynamicPropertyNameEntity>().HasOne(x => x.Property)
                        .WithMany(x => x.DisplayNames)
                        .HasForeignKey(x => x.PropertyId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DynamicPropertyNameEntity>()
                        .HasIndex(x => new { x.PropertyId, x.Locale, x.Name })
                        .HasName("IX_PlatformDynamicPropertyName_PropertyId_Locale_Name")
                        .IsUnique(true);


            modelBuilder.Entity<DynamicPropertyDictionaryItemEntity>().ToTable("PlatformDynamicPropertyDictionaryItem");
            modelBuilder.Entity<DynamicPropertyDictionaryItemEntity>().HasOne(x => x.Property)
                        .WithMany(x => x.DictionaryItems)
                        .HasForeignKey(x => x.PropertyId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DynamicPropertyDictionaryItemEntity>()
                        .HasIndex(x => new { x.PropertyId, x.Name })
                        .HasName("IX_PlatformDynamicPropertyDictionaryItem_PropertyId_Name")
                        .IsUnique(true);


            modelBuilder.Entity<DynamicPropertyDictionaryItemNameEntity>().ToTable("PlatformDynamicPropertyDictionaryItemName");
            modelBuilder.Entity<DynamicPropertyDictionaryItemNameEntity>().HasOne(x => x.DictionaryItem)
                        .WithMany(x => x.DisplayNames)
                        .HasForeignKey(x => x.DictionaryItemId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DynamicPropertyDictionaryItemNameEntity>()
                        .HasIndex(x => new { x.DictionaryItemId, x.Locale, x.Name })
                        .HasName("IX_PlatformDynamicPropertyDictionaryItemName_DictionaryItemId_Locale_Name")
                        .IsUnique(true);

            modelBuilder.Entity<DynamicPropertyObjectValueEntity>().ToTable("PlatformDynamicPropertyObjectValue");
            modelBuilder.Entity<DynamicPropertyObjectValueEntity>().HasOne(x => x.Property)
                        .WithMany(x => x.ObjectValues)
                        .HasForeignKey(x => x.PropertyId)
                        .OnDelete(DeleteBehavior.Cascade);            
            modelBuilder.Entity<DynamicPropertyObjectValueEntity>().HasOne(x => x.DictionaryItem)
                        .WithMany(x => x.ObjectValues)
                        .HasForeignKey(x => x.DictionaryItemId);
            modelBuilder.Entity<DynamicPropertyObjectValueEntity>().HasIndex(x => new { x.ObjectType, x.ObjectId })
                        .IsUnique(false)
                        .HasName("IX_ObjectType_ObjectId");        
            #endregion

            base.OnModelCreating(modelBuilder);
        }

    }
}
