using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.Platform.Data.Repositories
{
	public interface IPlatformRepository : IRepository
	{
		IQueryable<SettingEntity> Settings { get; }

        IQueryable<DynamicPropertyEntity> DynamicProperties { get; }
        IQueryable<DynamicPropertyDictionaryItemEntity> DynamicPropertyDictionaryItems { get; }
        IQueryable<DynamicPropertyObjectValueEntity> DynamicPropertyObjectValues { get; }
        IQueryable<OperationLogEntity> OperationLogs { get; }

        DynamicPropertyDictionaryItemEntity[] GetDynamicPropertyDictionaryItems(string propertyId);
		DynamicPropertyEntity[] GetDynamicPropertiesByIds(string[] ids);
		DynamicPropertyEntity[] GetDynamicPropertiesForType(string objectType);
		DynamicPropertyEntity[] GetObjectDynamicProperties(string[] objectTypes, string[] objectIds);

        SettingEntity GetSettingByName(string name);
        SettingEntity[] GetAllObjectSettings(string objectType, string objectId);
	}
}
