namespace VirtoCommerce.CatalogModule.Core2.Security
{
    public static class SecurityConstants
    {
        public static class Permissions
        {
            public const string Read = "catalog:read";
            public const string Create = "catalog:create";
            public const string Access = "catalog:access";
            public const string Update = "catalog:update";
            public const string Delete = "catalog:delete";
            public const string Export = "catalog:export";
            public const string Import = "catalog:import";
            public const string ReadBrowseFilters = "catalog:BrowseFilters:Read";
            public const string UpdateBrowseFilters = "catalog:BrowseFilters:Update";

            public static string[] AllPermissions = new[] { Read, Create, Access, Update, Delete, Export, Import, ReadBrowseFilters, UpdateBrowseFilters };
        }
    }
}
