using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Modularity;

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
                public static ModuleSetting PageSize = new ModuleSetting
                {
                    Name = "Inventory.ExportImport.PageSize",
                    ValueType = ModuleSetting.TypeInteger,
                    DefaultValue = "50",
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return PageSize;
                    }
                }
            }
        }
    }
}
