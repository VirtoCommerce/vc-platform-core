namespace VirtoCommerce.NotificationsModule.Core.Security
{
    public static class SecurityConstants
    {
        public static class Permissions
        {
            public const string Read = "notifications:read";
            public const string Create = "notifications:create";
            public const string Access = "notifications:access";
            public const string Update = "notifications:update";
            public const string Delete = "notifications:delete";
            public const string Export = "notifications:export";
            public const string Import = "notifications:import";
            public const string ReadTemplates = "notifications:templates:read";
            public const string CreateTemplate = "notifications:template:create";

            public static string[] AllPermissions = new[] { Read, Create, Access, Update, Delete, Export, Import, ReadTemplates, CreateTemplate };
        }
    }
}
