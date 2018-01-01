using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.Platform.Data.Repositories
{
    public class PlatformRepository : EFRepositoryBase, IPlatformRepository
    {
        public PlatformRepository(string nameOrConnectionString, params IInterceptor[] interceptors)
            : base(nameOrConnectionString, null, interceptors)
        {
            Database.SetInitializer<PlatformRepository>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
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

            modelBuilder.Entity<SettingValueEntity>().HasRequired(x => x.Setting)
                        .WithMany(x => x.SettingValues)
                        .HasForeignKey(x => x.SettingId)
                        .WillCascadeOnDelete(true);

            #endregion

            #region Dynamic Properties

            modelBuilder.Entity<DynamicPropertyEntity>().ToTable("PlatformDynamicProperty");
            modelBuilder.Entity<DynamicPropertyEntity>().HasIndex(x => new { x.ObjectType, x.Name })
                        .HasName("IX_PlatformDynamicProperty_ObjectType_Name")
                        .IsUnique(true);

            modelBuilder.Entity<DynamicPropertyNameEntity>().ToTable("PlatformDynamicPropertyName");
            modelBuilder.Entity<DynamicPropertyNameEntity>().HasRequired(x => x.Property)
                        .WithMany(x => x.DisplayNames)
                        .HasForeignKey(x => x.PropertyId)
                        .WillCascadeOnDelete(true); 
            modelBuilder.Entity<DynamicPropertyNameEntity>()
                        .HasIndex(x => new { x.PropertyId, x.Locale, x.Name })
                        .HasName("IX_PlatformDynamicPropertyName_PropertyId_Locale_Name")
                        .IsUnique(true);


            modelBuilder.Entity<DynamicPropertyDictionaryItemEntity>().ToTable("PlatformDynamicPropertyDictionaryItem");
            modelBuilder.Entity<DynamicPropertyDictionaryItemEntity>().HasRequired(x => x.Property)
                        .WithMany(x => x.DictionaryItems)
                        .HasForeignKey(x => x.PropertyId)
                         .WillCascadeOnDelete(true);
            modelBuilder.Entity<DynamicPropertyDictionaryItemEntity>()
                        .HasIndex(x => new { x.PropertyId, x.Name })
                        .HasName("IX_PlatformDynamicPropertyDictionaryItem_PropertyId_Name")
                        .IsUnique(true);


            modelBuilder.Entity<DynamicPropertyDictionaryItemNameEntity>().ToTable("PlatformDynamicPropertyDictionaryItemName");
            modelBuilder.Entity<DynamicPropertyDictionaryItemNameEntity>().HasRequired(x => x.DictionaryItem)
                        .WithMany(x => x.DisplayNames)
                        .HasForeignKey(x => x.DictionaryItemId)
                        .WillCascadeOnDelete(true);
            modelBuilder.Entity<DynamicPropertyDictionaryItemNameEntity>()
                        .HasIndex(x => new { x.DictionaryItemId, x.Locale, x.Name })
                        .HasName("IX_PlatformDynamicPropertyDictionaryItemName_DictionaryItemId_Locale_Name")
                        .IsUnique(true);

            modelBuilder.Entity<DynamicPropertyObjectValueEntity>().ToTable("PlatformDynamicPropertyObjectValue");
            modelBuilder.Entity<DynamicPropertyObjectValueEntity>().HasRequired(x => x.Property)
                        .WithMany(x => x.ObjectValues)
                        .HasForeignKey(x => x.PropertyId)
                        .WillCascadeOnDelete(true);
            modelBuilder.Entity<DynamicPropertyObjectValueEntity>().HasRequired(x => x.DictionaryItem)
                        .WithMany(x => x.ObjectValues)
                        .HasForeignKey(x => x.DictionaryItemId);
            modelBuilder.Entity<DynamicPropertyObjectValueEntity>().HasIndex(x => new { x.ObjectType, x.ObjectId })
                        .IsUnique(false)
                        .HasName("IX_ObjectType_ObjectId");
            #endregion

            #region Security

            // Tables
            modelBuilder.Entity<AccountEntity>().ToTable("PlatformAccount");
            modelBuilder.Entity<AccountEntity>().HasIndex(x => x.UserName).HasName("IX_UserName").IsUnique(true);


            modelBuilder.Entity<ApiAccountEntity>().ToTable("PlatformApiAccount");
            modelBuilder.Entity<ApiAccountEntity>().HasRequired(x => x.Account)
                        .WithMany(x => x.ApiAccounts)
                        .HasForeignKey(x => x.AccountId)
                        .WillCascadeOnDelete(true);
            modelBuilder.Entity<ApiAccountEntity>().HasIndex(x => x.AppId).HasName("IX_AppId").IsUnique(true);

            modelBuilder.Entity<RoleEntity>().ToTable("PlatformRole");

            modelBuilder.Entity<PermissionEntity>().ToTable("PlatformPermission");

            modelBuilder.Entity<RolePermissionEntity>().ToTable("PlatformRolePermission");
            modelBuilder.Entity<RolePermissionEntity>().HasRequired(x => x.Permission)
                        .WithMany(x => x.RolePermissions)
                        .HasForeignKey(x => x.PermissionId)
                        .WillCascadeOnDelete(true);

            modelBuilder.Entity<RolePermissionEntity>().HasRequired(x => x.Role)
                        .WithMany(x => x.RolePermissions)
                        .HasForeignKey(x => x.RoleId)
                        .WillCascadeOnDelete(true);

            modelBuilder.Entity<RoleAssignmentEntity>().ToTable("PlatformRoleAssignment");
            modelBuilder.Entity<RoleAssignmentEntity>().HasRequired(x => x.Account)
                        .WithMany(x => x.RoleAssignments)
                        .HasForeignKey(x => x.AccountId)
                        .WillCascadeOnDelete(true); 

            modelBuilder.Entity<RoleAssignmentEntity>().HasRequired(x => x.Role)
                        .WithMany()
                        .HasForeignKey(x => x.RoleId)
                        .WillCascadeOnDelete(true);

            modelBuilder.Entity<PermissionScopeEntity>().ToTable("PlatformPermissionScope");
            modelBuilder.Entity<PermissionScopeEntity>().HasRequired(x => x.RolePermission)
                        .WithMany(x => x.Scopes)
                        .HasForeignKey(x => x.RolePermissionId)
                        .WillCascadeOnDelete(true); ;
            #endregion



            base.OnModelCreating(modelBuilder);
        }

        #region IPlatformRepository Members
        public IQueryable<SettingEntity> Settings { get { return GetAsQueryable<SettingEntity>(); } }

        public IQueryable<DynamicPropertyEntity> DynamicProperties { get { return GetAsQueryable<DynamicPropertyEntity>(); } }
        public IQueryable<DynamicPropertyObjectValueEntity> DynamicPropertyObjectValues { get { return GetAsQueryable<DynamicPropertyObjectValueEntity>(); } }
        public IQueryable<DynamicPropertyDictionaryItemEntity> DynamicPropertyDictionaryItems { get { return GetAsQueryable<DynamicPropertyDictionaryItemEntity>(); } }

        public IQueryable<AccountEntity> Accounts { get { return GetAsQueryable<AccountEntity>(); } }
        public IQueryable<ApiAccountEntity> ApiAccounts { get { return GetAsQueryable<ApiAccountEntity>(); } }
        public IQueryable<RoleEntity> Roles { get { return GetAsQueryable<RoleEntity>(); } }
        public IQueryable<PermissionEntity> Permissions { get { return GetAsQueryable<PermissionEntity>(); } }
        public IQueryable<RoleAssignmentEntity> RoleAssignments { get { return GetAsQueryable<RoleAssignmentEntity>(); } }
        public IQueryable<RolePermissionEntity> RolePermissions { get { return GetAsQueryable<RolePermissionEntity>(); } }
        public IQueryable<OperationLogEntity> OperationLogs { get { return GetAsQueryable<OperationLogEntity>(); } }

        public RoleEntity GetRoleById(string roleId)
        {
            return Roles.Include(x => x.RolePermissions.Select(y => y.Permission))
                        .Include(x => x.RolePermissions.Select(y => y.Scopes))
                        .FirstOrDefault(x => x.Id == roleId);
        }

        public AccountEntity GetAccountByName(string userName, UserDetails detailsLevel)
        {
            var query = Accounts;

            if (detailsLevel == UserDetails.Full || detailsLevel == UserDetails.Export)
            {
                query = query
                    .Include(a => a.RoleAssignments.Select(ra => ra.Role.RolePermissions.Select(rp => rp.Permission)))
                    .Include(a => a.RoleAssignments.Select(ra => ra.Role.RolePermissions.Select(rp => rp.Scopes)))
                    .Include(a => a.ApiAccounts);
            }

            return query.FirstOrDefault(a => a.UserName == userName);
        }

        public DynamicPropertyDictionaryItemEntity[] GetDynamicPropertyDictionaryItems(string propertyId)
        {
            var retVal = DynamicPropertyDictionaryItems.Include(i => i.DisplayNames)
                                                       .Where(i => i.PropertyId == propertyId)
                                                       .ToArray();

            return retVal;
        }


        public DynamicPropertyEntity[] GetObjectDynamicProperties(string[] objectTypeNames, string[] objectIds)
        {
            var properties = DynamicProperties.Include(x => x.DisplayNames)
                                              .OrderBy(x => x.Name)
                                              .Where(x => objectTypeNames.Contains(x.ObjectType)).ToArray();

            var propertyIds = properties.Select(x => x.Id).ToArray();
            var proprValues = DynamicPropertyObjectValues.Include(x => x.DictionaryItem.DisplayNames)
                                                         .Where(x => propertyIds.Contains(x.PropertyId) && objectIds.Contains(x.ObjectId))
                                                         .ToArray();

            return properties;
        }

        public DynamicPropertyEntity[] GetDynamicPropertiesByIds(string[] ids)
        {
            var retVal = DynamicProperties.Include(x => x.DisplayNames)
                                          .Where(x => ids.Contains(x.Id))
                                          .OrderBy(x => x.Name)
                                          .ToArray();
            return retVal;
        }

        public DynamicPropertyEntity[] GetDynamicPropertiesForType(string objectType)
        {
            var retVal = DynamicProperties.Include(p => p.DisplayNames)
                                          .Where(p => p.ObjectType == objectType)
                                          .OrderBy(p => p.Name)
                                          .ToArray();
            return retVal;
        }

        public SettingEntity GetSettingByName(string name)
        {
            var result = Settings.Include(x => x.SettingValues).FirstOrDefault(x => x.Name == name && x.ObjectId == null && x.ObjectType == null);
            return result;
        }

        public SettingEntity[] GetAllObjectSettings(string objectType, string objectId)
        {
            var result = Settings.Include(x => x.SettingValues).Where(x => x.ObjectId == objectId && x.ObjectType == objectType).OrderBy(x => x.Name).ToArray();
            return result;
        }

        #endregion
      
    }
}
