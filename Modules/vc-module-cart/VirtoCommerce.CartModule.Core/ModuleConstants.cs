namespace VirtoCommerce.CartModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "cart:read";
                public const string Create = "cart:create";
                public const string Access = "cart:access";
                public const string Update = "cart:update";
                public const string Delete = "cart:delete";

                public static string[] AllPermissions = new[] { Read, Create, Access, Update, Delete };
            }
        }
    }
}
