using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ShippingModule.Core
{
    public class ModuleConstants
    {
        public static class Settings
        {
            public static class General
            {
                public static IEnumerable<SettingDescriptor> AllSettings => Enumerable.Empty<SettingDescriptor>();
            }

            public static IEnumerable<SettingDescriptor> AllSettings => General.AllSettings;
        }

    }
}
