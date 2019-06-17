using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Exceptions;

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

            //Deep load settings values for all object contains settings
            var hasSettingsObjects = entity.GetFlatObjectsListWithInterface<IHasSettings>();
            foreach (var hasSettingsObject in hasSettingsObjects)
            {
                var typeSettings = manager.GetSettingsForType(hasSettingsObject.TypeName);
                if (typeSettings.IsNullOrEmpty())
                {
                    throw new SettingsTypeNotRegisteredException(hasSettingsObject.TypeName);
                }
                hasSettingsObject.Settings = (await manager.GetObjectSettingsAsync(typeSettings.Select(x => x.Name), hasSettingsObject.TypeName, hasSettingsObject.Id)).ToList();
            }
        }

        public static async Task DeepSaveSettingsAsync(this ISettingsManager manager, IHasSettings entry)
        {
            await manager.DeepSaveSettingsAsync(new[] { entry });
        }
        /// <summary>
        /// Deep save entity and all nested objects settings values
        /// </summary>
        public static async Task DeepSaveSettingsAsync(this ISettingsManager manager, IEnumerable<IHasSettings> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            var forSaveSettings = new List<ObjectSettingEntry>();
            foreach (var entry in entries)
            {
                var haveSettingsObjects = entry.GetFlatObjectsListWithInterface<IHasSettings>();

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
            }
            if (forSaveSettings.Any())
            {
                await manager.SaveObjectSettingsAsync(forSaveSettings);
            }
        }

        /// <summary>
        /// Deep remove entity and all nested objects settings values
        /// </summary>
        public static async Task DeepRemoveSettingsAsync(this ISettingsManager manager, IHasSettings entry)
        {
            await manager.DeepRemoveSettingsAsync(new[] { entry });
        }

        /// <summary>
        /// Deep remove entity and all nested objects settings values
        /// </summary>
        public static async Task DeepRemoveSettingsAsync(this ISettingsManager manager, IEnumerable<IHasSettings> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }
            var foDeleteSettings = new List<ObjectSettingEntry>();
            foreach (var entry in entries)
            {
                var haveSettingsObjects = entry.GetFlatObjectsListWithInterface<IHasSettings>();
                foDeleteSettings.AddRange(haveSettingsObjects.SelectMany(x => x.Settings).Distinct());
            }
            await manager.RemoveObjectSettingsAsync(foDeleteSettings);
        }

        public static T GetValue<T>(this ISettingsManager manager, string settingName, T defaultValue)
        {
            return manager.GetValueAsync(settingName, defaultValue).GetAwaiter().GetResult();
        }

        public static async Task<T> GetValueAsync<T>(this ISettingsManager manager, string settingName, T defaultValue)
        {
            var result = defaultValue;
            ObjectSettingEntry objectSetting = null;

            try
            {
                objectSetting = await manager.GetObjectSettingAsync(settingName);
            }
            catch (PlatformException)
            {
                // This exception can be thrown when there is no setting registered with given name.
                // VC Platform 2.x was returning the default value in this case, so the platform 3.x will do the same.
            }

            if (objectSetting != null)
            {
                result = new List<ObjectSettingEntry> { objectSetting }.GetSettingValue(settingName, defaultValue);
            }

            return result;
        }

        public static void SetValue<T>(this ISettingsManager manager, string settingName, T value)
        {
            manager.SetValueAsync(settingName, new[] { value }).GetAwaiter().GetResult();
        }

        public static void SetValue<T>(this ISettingsManager manager, string settingName, T[] values)
        {
            manager.SetValueAsync(settingName, values).GetAwaiter().GetResult();
        }

        public static async Task SetValueAsync<T>(this ISettingsManager manager, string settingName, T value)
        {
            await manager.SetValueAsync(settingName, new[] { value });
        }

        public static async Task SetValueAsync<T>(this ISettingsManager manager, string settingName, T[] values)
        {
            var objectSetting = await manager.GetObjectSettingAsync(settingName);
            if (!objectSetting.IsMultiValue && values.Length > 1)
            {
                throw new Exception($"You can't save multiple values to none MultiValue settings, set {nameof(objectSetting.IsMultiValue)} to true");
            }

            objectSetting.Values = values.Cast<object>().ToArray();
            await manager.SaveObjectSettingsAsync(new[] { objectSetting });
        }

        public static T GetSettingValue<T>(this IEnumerable<ObjectSettingEntry> objectSettings, string settingName, T defaultValue)
        {
            var retVal = defaultValue;
            var setting = objectSettings.FirstOrDefault(x => x.Name.EqualsInvariant(settingName));

            if (setting != null)
            {
                var isCollection = typeof(IEnumerable).IsAssignableFrom(typeof(T));

                if (isCollection && !setting.IsMultiValue)
                {
                    throw new Exception("You can't get multiply values from single value setting");
                }

                if (setting.IsMultiValue)
                {
                    throw new Exception("You can't get single value from MultiValue setting");
                }

                if (!setting.Values.IsNullOrEmpty())
                {
                    if (setting.IsMultiValue)
                    {
                        retVal = (T)Convert.ChangeType(setting.Values, typeof(T), CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        retVal = (T)Convert.ChangeType(setting.Values.FirstOrDefault(), typeof(T), CultureInfo.InvariantCulture);
                    }
                }
            }

            return retVal;
        }
    }
}
