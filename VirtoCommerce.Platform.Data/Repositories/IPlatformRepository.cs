using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Assets;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.Platform.Data.Repositories
{
    public interface IPlatformRepository : IRepository
	{
	    IQueryable<AssetEntryEntity> AssetEntries { get; }
        IQueryable<SettingEntity> Settings { get; }

        IQueryable<DynamicPropertyEntity> DynamicProperties { get; }
        IQueryable<DynamicPropertyDictionaryItemEntity> DynamicPropertyDictionaryItems { get; }
        IQueryable<DynamicPropertyObjectValueEntity> DynamicPropertyObjectValues { get; }
        IQueryable<OperationLogEntity> OperationLogs { get; }

        Task<DynamicPropertyDictionaryItemEntity[]> GetDynamicPropertyDictionaryItemByIdsAsync(string[] ids);
        Task<DynamicPropertyEntity[]> GetDynamicPropertiesByIdsAsync(string[] ids);
        Task<DynamicPropertyEntity[]> GetObjectDynamicPropertiesAsync(string[] objectTypes, string[] objectIds);

	    Task<SettingEntity> GetSettingByNameAsync(string name);
	    Task<SettingEntity[]> GetAllObjectSettingsAsync(string objectType, string objectId);

	    AssetEntryEntity[] GetAssetsByIds(IEnumerable<string> ids);
    }
}
