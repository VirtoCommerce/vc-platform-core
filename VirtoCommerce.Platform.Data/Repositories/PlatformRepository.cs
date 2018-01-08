using System.Linq;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.Platform.Data.Repositories
{
    public class PlatformRepository : DbContextRepositoryBase<PlatformDbContext>, IPlatformRepository
    {
        public PlatformRepository(PlatformDbContext dbContext)
            :base(dbContext)
        {
        }

        #region IPlatformRepository Members
        public IQueryable<SettingEntity> Settings { get { return DbContext.Set<SettingEntity>(); } }

        public IQueryable<DynamicPropertyEntity> DynamicProperties { get { return DbContext.Set<DynamicPropertyEntity>(); } }
        public IQueryable<DynamicPropertyObjectValueEntity> DynamicPropertyObjectValues { get { return DbContext.Set<DynamicPropertyObjectValueEntity>(); } }
        public IQueryable<DynamicPropertyDictionaryItemEntity> DynamicPropertyDictionaryItems { get { return DbContext.Set<DynamicPropertyDictionaryItemEntity>(); } }

     
        public IQueryable<OperationLogEntity> OperationLogs { get { return DbContext.Set<OperationLogEntity>(); } }

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
