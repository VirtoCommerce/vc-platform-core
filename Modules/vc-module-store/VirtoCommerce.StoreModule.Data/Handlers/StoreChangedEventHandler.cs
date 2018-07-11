using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Commerce.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Events;

namespace VirtoCommerce.StoreModule.Data.Handlers
{
    public class StoreChangedEventHandler : IEventHandler<StoreChangedEvent>
    {
        private readonly ICommerceService _commerceService;
        private readonly ISettingsManager _settingManager;
        private readonly IDynamicPropertyService _dynamicPropertyService;

        public StoreChangedEventHandler(ICommerceService commerceService, ISettingsManager settingsManager, IDynamicPropertyService dynamicPropertyService)
        {
            _commerceService = commerceService;
            _settingManager = settingsManager;
            _dynamicPropertyService = dynamicPropertyService;
        }

        public virtual async Task Handle(StoreChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                if (changedEntry.EntryState == EntryState.Added)
                {
                    var taskUpsertSeoForObjects = _commerceService.UpsertSeoForObjectsAsync(new[] { changedEntry.NewEntry });
                    var taskSaveDynamicPropertyValues = _dynamicPropertyService.SaveDynamicPropertyValuesAsync(changedEntry.NewEntry);
                    var taskSaveEntitySettingsValues = _settingManager.DeepSaveSettingsAsync(changedEntry.NewEntry);
                    await Task.WhenAll(taskUpsertSeoForObjects, taskSaveDynamicPropertyValues, taskSaveEntitySettingsValues);
                }
                else if (changedEntry.EntryState == EntryState.Modified)
                {
                    var taskUpsertSeoForObjects = _commerceService.UpsertSeoForObjectsAsync(new[] { changedEntry.NewEntry });
                    var taskSaveDynamicPropertyValues = _dynamicPropertyService.SaveDynamicPropertyValuesAsync(changedEntry.NewEntry);
                    var taskSaveEntitySettingsValues = _settingManager.DeepSaveSettingsAsync(changedEntry.NewEntry);
                    await Task.WhenAll(taskUpsertSeoForObjects, taskSaveDynamicPropertyValues, taskSaveEntitySettingsValues);
                }
                else if (changedEntry.EntryState == EntryState.Deleted)
                {
                    var taskDeleteSeoForObject = _commerceService.DeleteSeoForObjectAsync(changedEntry.NewEntry);
                    var taskDeleteDynamicPropertyValues = _dynamicPropertyService.DeleteDynamicPropertyValuesAsync(changedEntry.NewEntry);
                    var taskRemoveEntitySettingsValues = _settingManager.DeepRemoveSettingsAsync(changedEntry.NewEntry);
                    await Task.WhenAll(taskDeleteSeoForObject, taskDeleteDynamicPropertyValues, taskRemoveEntitySettingsValues);
                }
            }
        }
    }
}
