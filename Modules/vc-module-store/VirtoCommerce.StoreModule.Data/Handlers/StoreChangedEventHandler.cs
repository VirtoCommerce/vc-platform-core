using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Events;

namespace VirtoCommerce.StoreModule.Data.Handlers
{
    public class StoreChangedEventHandler : IEventHandler<StoreChangedEvent>
    {
        private readonly ISettingsManager _settingManager;
        private readonly IDynamicPropertyService _dynamicPropertyService;

        public StoreChangedEventHandler(ISettingsManager settingsManager, IDynamicPropertyService dynamicPropertyService)
        {
            _settingManager = settingsManager;
            _dynamicPropertyService = dynamicPropertyService;
        }

        public virtual async Task Handle(StoreChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                if (changedEntry.EntryState == EntryState.Added)
                {
                    var taskSaveDynamicPropertyValues = _dynamicPropertyService.SaveDynamicPropertyValuesAsync(changedEntry.NewEntry);
                    var taskSaveEntitySettingsValues = _settingManager.DeepSaveSettingsAsync(changedEntry.NewEntry);
                    await Task.WhenAll(taskSaveDynamicPropertyValues, taskSaveEntitySettingsValues);
                }
                else if (changedEntry.EntryState == EntryState.Modified)
                {
                    var taskSaveDynamicPropertyValues = _dynamicPropertyService.SaveDynamicPropertyValuesAsync(changedEntry.NewEntry);
                    var taskSaveEntitySettingsValues = _settingManager.DeepSaveSettingsAsync(changedEntry.NewEntry);
                    await Task.WhenAll(taskSaveDynamicPropertyValues, taskSaveEntitySettingsValues);
                }
                else if (changedEntry.EntryState == EntryState.Deleted)
                {
                    var taskDeleteDynamicPropertyValues = _dynamicPropertyService.DeleteDynamicPropertyValuesAsync(changedEntry.NewEntry);
                    var taskRemoveEntitySettingsValues = _settingManager.DeepRemoveSettingsAsync(changedEntry.NewEntry);
                    await Task.WhenAll(taskDeleteDynamicPropertyValues, taskRemoveEntitySettingsValues);
                }
            }
        }
    }
}
