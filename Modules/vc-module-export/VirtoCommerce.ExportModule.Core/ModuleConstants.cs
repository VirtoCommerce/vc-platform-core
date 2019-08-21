using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ExportModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Access = "export:access";
                public const string Download = "export:download";

                public static readonly string[] AllPermissions = { Access, Download };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor ExportFileNameTemplate = new SettingDescriptor
                {
                    Name = "Export.FileNameTemplate",
                    ValueType = SettingValueType.ShortText,
                    GroupName = "Export|General",
                    DefaultValue = "export_{0:yyyyMMddHHmmss}"
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return ExportFileNameTemplate;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings => General.AllSettings;
        }
    }
}
