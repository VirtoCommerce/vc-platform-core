namespace VirtoCommerce.SitemapsModule.Core.ModuleConstants
{
    public static class ModulePermissions
    {
        public const string Access = "sitemaps:access",
            Create = "sitemaps:create",
            Read = "sitemaps:read",
            Update = "sitemaps:update",
            Delete = "sitemaps:delete";

        public static readonly string[] AllPermissions = { Access, Create, Read, Update, Delete };
    }
}
