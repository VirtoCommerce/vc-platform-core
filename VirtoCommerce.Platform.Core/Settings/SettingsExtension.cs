using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Settings
{
    public static class SettingsExtension
    {
        /// <summary>
        /// Deep load and populate settings values for entity and all nested objects 
        /// </summary>
        /// <param name="entity"></param>
        public static async Task DeepLoadSettingsAsync(this ISettingsManager manager, IHasSettings entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentException("entity must have Id");
            }

            //Deep load settings values for all object contains settings
            var hasSettingsObjects = entity.GetFlatObjectsListWithInterface<IHasSettings>();
            foreach (var hasSettingsObject in hasSettingsObjects.Where(x => x.Settings != null))
            {
                var typeSettings = manager.GetSettingsForType(hasSettingsObject.TypeName);
                if (!typeSettings.IsNullOrEmpty())
                {
                    hasSettingsObject.Settings = (await manager.GetObjectSettingsAsync(typeSettings.Select(x => x.Name), hasSettingsObject.TypeName, hasSettingsObject.Id)).ToList();
                }
            }
        }

        /// <summary>
        /// Deep save entity and all nested objects settings values
        /// </summary>
        /// <param name="entity"></param>
        public static async Task DeepSaveSettingsAsync(this ISettingsManager manager, IHasSettings entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentException("entity must have Id");
            }

            var forSaveSettings = new List<ObjectSettingEntry>();
            var haveSettingsObjects = entity.GetFlatObjectsListWithInterface<IHasSettings>();

            foreach (var haveSettingsObject in haveSettingsObjects.Where(x => x.Settings != null))
            {
                //Save settings
                foreach (var setting in haveSettingsObject.Settings)
                {
                    setting.ObjectId = haveSettingsObject.Id;
                    setting.ObjectType = haveSettingsObject.TypeName;
                    forSaveSettings.Add(setting);
                }
            }
            if (forSaveSettings.Any())
            {
                await manager.SaveObjectSettingsAsync(forSaveSettings);
            }
        }
        /// <summary>
        /// Deep remove entity and all nested objects settings values
        /// </summary>
        /// <param name="entity"></param>
		public static async Task DeepRemoveSettingsAsync(this ISettingsManager manager, IHasSettings entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentException("entity must have Id");
            }
            var haveSettingsObjects = entity.GetFlatObjectsListWithInterface<IHasSettings>();
            await manager.RemoveObjectSettingsAsync(haveSettingsObjects.SelectMany(x => x.Settings).Distinct());
        }

        public static T GetValue<T>(this ISettingsManager manager, string name, T defaultValue)
        {
            return manager.GetValueAsync(name, defaultValue).GetAwaiter().GetResult();
        }

        public static async Task<T> GetValueAsync<T>(this ISettingsManager manager, string name, T defaultValue)
        {
            var result = defaultValue;

            var objectSetting = await manager.GetObjectSettingAsync(name);
            if (objectSetting.Value != null)
            {
                result = (T)objectSetting.Value;
            }
            return result;
        }

        public static void SetValue<T>(this ISettingsManager manager, string name, T value)
        {
            manager.SetValueAsync(name, value).GetAwaiter().GetResult();
        }

        public static async Task SetValueAsync<T>(this ISettingsManager manager, string name, T value)
        {
            var type = typeof(T);
            var objectSetting = await manager.GetObjectSettingAsync(name);
            objectSetting.Value = value;
            await manager.SaveObjectSettingsAsync(new[] { objectSetting });
        }

        public static T GetSettingValue<T>(this IEnumerable<ObjectSettingEntry> objectSettings, string settingName, T defaulValue)
        {
            var retVal = defaulValue;
            var setting = objectSettings.FirstOrDefault(x => x.Name.EqualsInvariant(settingName));
            if (setting != null && setting.Value != null)
            {
                retVal = (T)Convert.ChangeType(setting.Value, typeof(T), CultureInfo.InvariantCulture);
            }
            return retVal;
        }
    }
}
