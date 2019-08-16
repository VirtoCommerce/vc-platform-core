using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.InventoryModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string FulfillmentEdit = "inventory:fulfillment:edit";
                public const string FulfillmentDelete = "inventory:fulfillment:delete";
                public const string Read = "inventory:read";
                public const string Create = "inventory:create";
                public const string Update = "inventory:update";
                public const string Access = "inventory:access";
                public const string Delete = "inventory:delete";

                public static string[] AllPermissions = new[] { FulfillmentEdit, FulfillmentDelete, Read, Create, Update, Access, Delete };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor PageSize = new SettingDescriptor
                {
                    Name = "Inventory.ExportImport.PageSize",
                    GroupName = "Inventory | General",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = "50",
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return PageSize;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings
            {
                get
                {
                    return General.AllSettings;
                }
            }
        }
    }
}
