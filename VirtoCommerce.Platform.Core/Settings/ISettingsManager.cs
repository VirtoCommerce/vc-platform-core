using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Settings
{
    public interface ISettingsManager : ISettingsRegistrar
    {
        Task<ObjectSettingEntry> GetObjectSettingAsync(string name, string objectType = null, string objectId = null);
        Task<IEnumerable<ObjectSettingEntry>> GetObjectSettingsAsync(IEnumerable<string> names, string objectType = null, string objectId = null);
        Task SaveObjectSettingsAsync(IEnumerable<ObjectSettingEntry> objectSettings);
        Task RemoveObjectSettingsAsync(IEnumerable<ObjectSettingEntry> objectSettings);
    }
}
