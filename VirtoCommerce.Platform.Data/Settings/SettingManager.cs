using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Model;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.Platform.Data.Settings
{
    /// <summary>
    /// Provide next functionality to working with settings
    /// - Load setting metainformation from module manifest and database 
    /// - Deep load all settings for entity
    /// - Mass update all entity settings
    /// </summary>
    public class SettingsManager : ISettingsManager
    {
        private readonly IModuleCatalog _moduleCatalog;
        private readonly Func<IPlatformRepository> _repositoryFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly IDictionary<string, List<SettingEntry>> _runtimeModuleSettingsMap = new Dictionary<string, List<SettingEntry>>();

        public SettingsManager(ILocalModuleCatalog moduleCatalog, Func<IPlatformRepository> repositoryFactory, IMemoryCache memoryCache)
        {
            _moduleCatalog = moduleCatalog;
            _repositoryFactory = repositoryFactory;
            _memoryCache = memoryCache;
        }

        #region ISettingsManager Members

        public ManifestModuleInfo[] GetModules()
        {
            var retVal = GetModulesWithSettings().ToArray();
            return retVal;
        }

        public SettingEntry GetSettingByName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            var cacheKey = CacheKey.With(GetType(), "GetSettingByName", name);
            var result = _memoryCache.GetOrCreateExclusive(cacheKey, (cacheEntry) =>
            {
                SettingEntry setting = null;
                //Get setting definition from module manifest first
                var moduleSetting = GetModuleSettingByName(name);
                if (moduleSetting != null)
                {
                    setting = moduleSetting.ToSettingEntry();
                }
                using (var repository = _repositoryFactory())
                {
                    //try to load setting from db
                    var settingEntity = repository.GetSettingByName(name);
                    if (settingEntity != null)
                    {
                        setting = settingEntity.ToModel(setting ?? AbstractTypeFactory<SettingEntry>.TryCreateInstance());
                    }
                }
                //Create new setting for unregistered setting name
                if (setting == null)
                {
                    setting = AbstractTypeFactory<SettingEntry>.TryCreateInstance();
                    setting.Name = name;
                }
                //Add cache  expiration token for setting
                cacheEntry.AddExpirationToken(SettingsCacheRegion.CreateChangeToken(setting));

                return setting;
            });
            return result;
        }

        public void LoadEntitySettingsValues(IHaveSettings entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrEmpty(entity.Id))
                throw new ArgumentException("entity must have Id");

            //Deep load settings values for all object contains settings
            var haveSettingsObjects = entity.GetFlatObjectsListWithInterface<IHaveSettings>();

            foreach (var haveSettingsObject in haveSettingsObjects)
            {
                var cacheKey = CacheKey.With(GetType(), "LoadEntitySettingsValues", entity.TypeName, entity.Id);
                var storedSettings = _memoryCache.GetOrCreateExclusive(cacheKey, (cacheEntry) =>
               {
                   var settingEntries = new List<SettingEntry>();
                   using (var repository = _repositoryFactory())
                   {
                       foreach (var settingEntry in repository.GetAllObjectSettings(entity.TypeName, entity.Id).Select(x => x.ToModel(AbstractTypeFactory<SettingEntry>.TryCreateInstance())))
                       {
                           //Add cache  expiration token for setting
                           cacheEntry.AddExpirationToken(SettingsCacheRegion.CreateChangeToken(settingEntry));
                           settingEntries.Add(settingEntry);
                       }
                       return settingEntries;
                   }
               });

                // Replace settings values with stored in database
                if (haveSettingsObject.Settings != null)
                {
                    //Need clone settings entry because it may be shared for multiple instances
                    haveSettingsObject.Settings = haveSettingsObject.Settings.Select(x => (SettingEntry)x.Clone()).ToList();

                    foreach (var setting in haveSettingsObject.Settings)
                    {
                        var storedSetting = storedSettings.Find(x => x == setting);
                        //First try to used stored object setting values
                        if (storedSetting != null)
                        {
                            setting.Value = storedSetting.Value;
                            setting.ArrayValues = storedSetting.ArrayValues;
                        }
                        else if (setting.Value == null && setting.ArrayValues == null)
                        {
                            //try to use global setting value
                            var globalSetting = GetSettingByName(setting.Name);
                            var defaultValue = (globalSetting ?? setting).DefaultValue;

                            if (setting.IsArray)
                            {
                                setting.ArrayValues = globalSetting?.ArrayValues ?? new[] { defaultValue };
                            }
                            else
                            {
                                setting.Value = globalSetting?.Value ?? defaultValue;
                            }
                        }
                    }
                }
            }
        }

        public void SaveEntitySettingsValues(IHaveSettings entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrEmpty(entity.Id))
                throw new ArgumentException("entity must have Id");

            var haveSettingsObjects = entity.GetFlatObjectsListWithInterface<IHaveSettings>();

            foreach (var haveSettingsObject in haveSettingsObjects)
            {
                var settings = new List<SettingEntry>();

                if (haveSettingsObject.Settings != null)
                {
                    //Save settings
                    foreach (var setting in haveSettingsObject.Settings)
                    {
                        setting.ObjectId = haveSettingsObject.Id;
                        setting.ObjectType = haveSettingsObject.TypeName;
                        settings.Add(setting);
                    }
                }
                SaveSettings(settings.ToArray());
            }
        }

        public void RemoveEntitySettings(IHaveSettings entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (string.IsNullOrEmpty(entity.Id))
                throw new ArgumentException("entity must have Id");

            using (var repository = _repositoryFactory())
            {
                var settings = repository.GetAllObjectSettings(entity.TypeName, entity.Id).ToArray();
                foreach (var setting in settings)
                {
                    repository.Remove(setting);
                }
                repository.UnitOfWork.Commit();
                ClearCache(settings.Select(x => x.ToModel(AbstractTypeFactory<SettingEntry>.TryCreateInstance())).ToArray());
            }
        }

        public SettingEntry[] GetModuleSettings(string moduleId)
        {
            var result = new List<SettingEntry>();

            var moduleManifest = GetModulesWithSettings().FirstOrDefault(m => m.Id == moduleId);

            if (moduleManifest != null && moduleManifest.Settings != null && moduleManifest.Settings.Any())
            {
                //Load settings from requested module manifest with values from database
                foreach (var group in moduleManifest.Settings)
                {
                    if (group.Settings != null)
                    {
                        foreach (var setting in group.Settings)
                        {
                            var settingEntry = GetSettingByName(setting.Name);
                            settingEntry.GroupName = group.Name;
                            settingEntry.ModuleId = moduleId;
                            result.Add(settingEntry);
                        }
                    }
                }
                //Try add runtime defined settings for requested module
                if (!string.IsNullOrEmpty(moduleId))
                {
                    if (_runtimeModuleSettingsMap.TryGetValue(moduleId, out var runtimeSettings))
                    {
                        result.AddRange(runtimeSettings);
                    }
                }
            }
            return result.OrderBy(x => x.Name).ToArray();
        }

        /// <summary>
        /// Register module settings runtime
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="settings"></param>
        public void RegisterModuleSettings(string moduleId, params SettingEntry[] settings)
        {
            //check module exist
            if (!GetModules().Any(x => x.Id == moduleId))
            {
                throw new ArgumentException(moduleId + " not exist");
            }
            List<SettingEntry> moduleSettings;
            if (!_runtimeModuleSettingsMap.TryGetValue(moduleId, out moduleSettings))
            {
                moduleSettings = new List<SettingEntry>();
                _runtimeModuleSettingsMap[moduleId] = moduleSettings;
            }
            foreach (var setting in settings)
            {
                var clonedSetting = setting.Clone() as SettingEntry;
                clonedSetting.IsRuntime = true;
                moduleSettings.Add(clonedSetting);
            }
        }

        public void SaveSettings(SettingEntry[] settings)
        {
            if (settings != null && settings.Any())
            {
                using (var repository = _repositoryFactory())
                {
                    var settingNames = settings.Select(x => x.Name).Distinct().ToArray();
                    var alreadyExistSettings = repository.Settings
                        .Include(s => s.SettingValues)
                        //We need to find settings DB records matched for given settings by several properties and not only the Id
                        //to do that we use filtration only by name (due to performance reasons)
                        .Where(x => settingNames.Contains(x.Name))
                        .ToList()
                        //then we need convert resulting DB entities to model for compare with given settings 
                        .Where(x => settings.Contains(x.ToModel(AbstractTypeFactory<SettingEntry>.TryCreateInstance())))
                        .ToList();

                    var target = new { Settings = new ObservableCollection<SettingEntity>(alreadyExistSettings) };
                    var source = new { Settings = new ObservableCollection<SettingEntity>(settings.Select(x => AbstractTypeFactory<SettingEntity>.TryCreateInstance().FromModel(x))) };

                    var settingComparer = AnonymousComparer.Create((SettingEntity x) => string.Join("-", x.Name, x.ObjectType, x.ObjectId));
                    source.Settings.Patch(target.Settings, settingComparer, (sourceSetting, targetSetting) => sourceSetting.Patch(targetSetting));

                    repository.UnitOfWork.Commit();
                }

                ClearCache(settings);
            }
        }

        public T[] GetArray<T>(string name, T[] defaultValue)
        {
            var result = defaultValue;

            var setting = GetSettingByName(name);

            if (setting != null)
            {
                if (!setting.RawArrayValues.IsNullOrEmpty())
                {
                    result = setting.RawArrayValues.Cast<T>().ToArray();
                }
                else if (setting.RawValue != null)
                {
                    result = new[] { (T)setting.RawValue };
                }
                else if (setting.RawDefaultValue != null)
                {
                    result = new[] { (T)setting.RawDefaultValue };
                }
            }

            return result;
        }

        public T GetValue<T>(string name, T defaultValue)
        {
            var result = defaultValue;

            var values = GetArray(name, new[] { defaultValue });

            if (values.Any())
            {
                result = values.First();
            }

            return result;
        }

        public void SetValue<T>(string name, T value)
        {
            var type = typeof(T);
            var setting = AbstractTypeFactory<SettingEntry>.TryCreateInstance();
            setting.Name = name;

            if (type.IsArray)
            {
                setting.IsArray = true;
                setting.ValueType = type.GetElementType().ToSettingValueType();
                if (value != null)
                {
                    setting.ArrayValues = ((IEnumerable)value).OfType<object>()
                                        .Select(v => v == null ? null : string.Format(CultureInfo.InvariantCulture, "{0}", v))
                                        .ToArray();
                }
            }
            else
            {
                setting.ValueType = type.ToSettingValueType();
                setting.Value = value == null ? null : string.Format(CultureInfo.InvariantCulture, "{0}", value);
            }
            SaveSettings(new[] { setting });
        }

        #endregion

        private void ClearCache(SettingEntry[] settings)
        {
            //Clear setting from cache
            foreach (var setting in settings)
            {
                SettingsCacheRegion.ExpireSetting(setting);
            }
        }

        private IEnumerable<ManifestModuleInfo> GetModulesWithSettings()
        {
            return _moduleCatalog.Modules.OfType<ManifestModuleInfo>()
                                 .Where(m => !m.Settings.IsNullOrEmpty());
        }

        private ModuleSetting GetModuleSettingByName(string name)
        {
            return GetAllModulesSettings().FirstOrDefault(s => s.Name == name);
        }

        private IEnumerable<ModuleSetting> GetAllModulesSettings()
        {
            return GetModulesWithSettings().SelectMany(m => m.Settings)
                .Where(g => g.Settings != null).SelectMany(g => g.Settings);
        }
    }
}
