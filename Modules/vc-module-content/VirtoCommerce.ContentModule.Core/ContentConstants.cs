using System.Collections.Generic;
using VirtoCommerce.ContentModule.Web.Model;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core
{
    public static class ContentConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "content:read",
                    Access = "content:access",
                    Create = "content:create",
                    Delete = "content:delete",
                    Update = "content:update";

                public static string[] AllPermissions = new[] { Read, Access, Create, Delete, Update };

            }
        }
    }
}
