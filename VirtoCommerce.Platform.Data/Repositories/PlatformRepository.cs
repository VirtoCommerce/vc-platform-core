using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Assets;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.Platform.Data.Repositories
{
    public class PlatformRepository : DbContextRepositoryBase<PlatformDbContext>, IPlatformRepository
    {
        public PlatformRepository(PlatformDbContext dbContext)
            : base(dbContext)
        {
        }

        #region IPlatformRepository Members
        public IQueryable<SettingEntity> Settings { get { return DbContext.Set<SettingEntity>(); } }

        public IQueryable<DynamicPropertyEntity> DynamicProperties { get { return DbContext.Set<DynamicPropertyEntity>(); } }
        public IQueryable<DynamicPropertyObjectValueEntity> DynamicPropertyObjectValues { get { return DbContext.Set<DynamicPropertyObjectValueEntity>(); } }
        public IQueryable<DynamicPropertyDictionaryItemEntity> DynamicPropertyDictionaryItems { get { return DbContext.Set<DynamicPropertyDictionaryItemEntity>(); } }


        public IQueryable<OperationLogEntity> OperationLogs { get { return DbContext.Set<OperationLogEntity>(); } }



        public async Task<DynamicPropertyEntity[]> GetObjectDynamicPropertiesAsync(string[] objectTypeNames, string[] objectIds)
        {
            var properties = DynamicProperties.Include(x => x.DisplayNames)
                                              .OrderBy(x => x.Name)
                                              .Where(x => objectTypeNames.Contains(x.ObjectType)).ToArray();

            var propertyIds = properties.Select(x => x.Id).ToArray();
            var proprValues = await DynamicPropertyObjectValues.Include(x => x.DictionaryItem.DisplayNames)
                                                         .Where(x => propertyIds.Contains(x.PropertyId) && objectIds.Contains(x.ObjectId))
                                                         .ToArrayAsync();

            return properties;
        }

        public async Task<DynamicPropertyDictionaryItemEntity[]> GetDynamicPropertyDictionaryItemByIdsAsync(string[] ids)
        {
            var retVal = await DynamicPropertyDictionaryItems.Include(x => x.DisplayNames)
                                     .Where(x => ids.Contains(x.Id))
                                     .ToArrayAsync();
            return retVal;
        }

        public async Task<DynamicPropertyEntity[]> GetDynamicPropertiesByIdsAsync(string[] ids)
        {
            var retVal = await DynamicProperties.Include(x => x.DisplayNames)
                                          .Where(x => ids.Contains(x.Id))
                                          .OrderBy(x => x.Name)
                                          .ToArrayAsync();
            return retVal;
        }



        public async Task<SettingEntity> GetSettingByNameAsync(string name)
        {
            var result = await Settings.Include(x => x.SettingValues)
                                       .FirstOrDefaultAsync(x => x.Name == name && x.ObjectId == null && x.ObjectType == null);
            return result;
        }

        public async Task<SettingEntity[]> GetAllObjectSettingsAsync(string objectType, string objectId)
        {
            var result = await Settings.Include(x => x.SettingValues)
                                 .Where(x => x.ObjectId == objectId && x.ObjectType == objectType)
                                 .OrderBy(x => x.Name)
                                 .ToArrayAsync();
            return result;
        }

        public IQueryable<AssetEntryEntity> AssetEntries => DbContext.Set<AssetEntryEntity>();

        public AssetEntryEntity[] GetAssetsByIds(IEnumerable<string> ids)
        {
            return AssetEntries.Where(x => ids.Contains(x.Id)).ToArray();
        }


        #endregion

    }
}
