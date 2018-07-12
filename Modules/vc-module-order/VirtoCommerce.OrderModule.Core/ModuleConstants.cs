using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.OrderModule.Core
{
    public class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "order:read";
                public const string Create = "order:create";
                public const string Update = "order:update";
                public const string Access = "order:access";
                public const string Delete = "order:delete";

                public static string[] AllPermissions = new[] { Read, Create, Update, Access, Delete };
            }
        }
    }
}
