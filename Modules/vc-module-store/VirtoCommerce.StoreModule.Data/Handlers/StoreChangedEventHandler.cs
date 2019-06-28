using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Events;
using VirtoCommerce.StoreModule.Core.Model;

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

        public virtual Task Handle(StoreChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                if (changedEntry.EntryState == EntryState.Added)
                {
                    BackgroundJob.Enqueue(() => SaveDynamicPropertyValuesInBackground(changedEntry.NewEntry));
                }
                else if (changedEntry.EntryState == EntryState.Modified)
                {
                    BackgroundJob.Enqueue(() => SaveDynamicPropertyValuesInBackground(changedEntry.NewEntry));
                }
                else if (changedEntry.EntryState == EntryState.Deleted)
                {
                    BackgroundJob.Enqueue(() => DeleteDynamicPropertyValuesInBackground(changedEntry.NewEntry));
                }
            }

            return Task.CompletedTask;
        }


        [DisableConcurrentExecution(60 * 60 * 24)]
        public void SaveDynamicPropertyValuesInBackground(Store entry)
        {
            SaveDynamicPropertyValuesAsync(entry).GetAwaiter().GetResult();
        }

        protected virtual async Task SaveDynamicPropertyValuesAsync(Store entry)
        {
            //TODO
            await _settingManager.DeepSaveSettingsAsync(entry);
            //var taskSaveDynamicPropertyValues = _dynamicPropertyService.SaveDynamicPropertyValuesAsync(entry);
            //var taskSaveEntitySettingsValues = _settingManager.DeepSaveSettingsAsync(entry);
            //await Task.WhenAll(taskSaveDynamicPropertyValues, taskSaveEntitySettingsValues);
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public void DeleteDynamicPropertyValuesInBackground(Store entry)
        {
            DeleteDynamicPropertyValuesAsync(entry).GetAwaiter().GetResult();
        }

        protected virtual async Task DeleteDynamicPropertyValuesAsync(Store entry)
        {
            //TODO
            await _settingManager.DeepRemoveSettingsAsync(entry);
            //var taskDeleteDynamicPropertyValues = _dynamicPropertyService.DeleteDynamicPropertyValuesAsync(entry);
            //var taskRemoveEntitySettingsValues = _settingManager.DeepRemoveSettingsAsync(entry);
            //await Task.WhenAll(taskDeleteDynamicPropertyValues, taskRemoveEntitySettingsValues);
        }
    }
}
