using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.ImageToolsModule.Core
{
    public static class ThumbnailConstants
    {
        public static class Permissions
        {
            public static class Security
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
                public static ModuleSetting EnableImageProcessJob = new ModuleSetting
                {
                    Name = "ImageTools.Thumbnails.EnableImageProcessJob",
                    ValueType = ModuleSetting.TypeBoolean,
                    DefaultValue = false.ToString(),
                };

                public static ModuleSetting ImageProcessJobCronExpression = new ModuleSetting
                {
                    Name = "ImageTools.Thumbnails.ImageProcessJobCronExpression",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = "0 0 * * *"
                };

                public static ModuleSetting ProcessBacthSize = new ModuleSetting
                {
                    Name = "ImageTools.Thumbnails.ProcessBacthSize",
                    ValueType = ModuleSetting.TypeInteger,
                    DefaultValue = "50"
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return EnableImageProcessJob;
                        yield return ImageProcessJobCronExpression;
                        yield return ProcessBacthSize;
                    }
                }
            }
        }
    }
}
