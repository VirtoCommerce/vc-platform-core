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
