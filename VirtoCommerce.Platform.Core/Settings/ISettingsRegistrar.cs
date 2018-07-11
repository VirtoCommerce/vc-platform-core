using System.Collections.Generic;

namespace VirtoCommerce.Platform.Core.Settings
{
    public interface ISettingsRegistrar
    {
        IEnumerable<SettingDescriptor> AllRegisteredSettings { get; }
        /// <summary>
        /// Register new setting 
        /// </summary>
        void RegisterSettings(IEnumerable<SettingDescriptor> settings, string moduleId = null);
    }
}
