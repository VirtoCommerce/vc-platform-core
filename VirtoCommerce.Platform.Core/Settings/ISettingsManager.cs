using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Settings
{
    public interface ISettingsManager
    {
        /// <summary>
        /// Deep load and populate settings values for entity and all nested objects 
        /// </summary>
        /// <param name="entity"></param>
		Task LoadEntitySettingsValuesAsync(IHaveSettings entity);
        /// <summary>
        /// Deep save entity and all nested objects settings values
        /// </summary>
        /// <param name="entity"></param>
        Task SaveEntitySettingsValuesAsync(IHaveSettings entity);
        /// <summary>
        /// Deep remove entity and all nested objects settings values
        /// </summary>
        /// <param name="entity"></param>
		Task RemoveEntitySettingsAsync(IHaveSettings entity);
        Task<SettingEntry> GetSettingByNameAsync(string name);
        Task<SettingEntry[]> GetModuleSettingsAsync(string moduleId);
        Task SaveSettingsAsync(SettingEntry[] settings);
        /// <summary>
        /// Used to runtime settings registration
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="settings"></param>
        Task RegisterModuleSettingsAsync(string moduleId, params SettingEntry[] settings);

        T GetValue<T>(string name, T defaultValue);
        Task<T> GetValueAsync<T>(string name, T defaultValue);
        T[] GetArray<T>(string name, T[] defaultValue);
        Task<T[]> GetArrayAsync<T>(string name, T[] defaultValue);
        void SetValue<T>(string name, T value);
        Task SetValueAsync<T>(string name, T value);
    }
}
