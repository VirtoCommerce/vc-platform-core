using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ImageToolsModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Access = "thumbnail:access",
                    Create = "thumbnail:create",
                    Delete = "thumbnail:delete",
                    Update = "thumbnail:update",
                    Read = "thumbnail:read";

                public static string[] AllPermissions = new[] { Access, Create, Delete, Update, Read };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor EnableImageProcessJob = new SettingDescriptor
                {
                    Name = "ImageTools.Thumbnails.EnableImageProcessJob",
                    GroupName = "Thumbnail|General",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false,
                    RestartRequired = true
                };

                public static SettingDescriptor ImageProcessJobCronExpression = new SettingDescriptor
                {
                    Name = "ImageTools.Thumbnails.ImageProcessJobCronExpression",
                    GroupName = "Thumbnail|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "0 0 * * *",
                    RestartRequired = true
                };

                public static SettingDescriptor ProcessBatchSize = new SettingDescriptor
                {
                    Name = "ImageTools.Thumbnails.ProcessBatchSize",
                    GroupName = "Thumbnail|General",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = "50"
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return EnableImageProcessJob;
                        yield return ImageProcessJobCronExpression;
                        yield return ProcessBatchSize;
                    }
                }
            }
            public static IEnumerable<SettingDescriptor> AllSettings => General.AllSettings;
        }
    }
}
