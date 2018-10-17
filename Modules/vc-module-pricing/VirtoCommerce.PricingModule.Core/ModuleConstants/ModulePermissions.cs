namespace VirtoCommerce.PricingModule.Core.ModuleConstants
{
    public static class ModulePermissions
    {
        public const string Read = "pricing:read",
            Create = "pricing:create",
            Access = "pricing:access",
            Update = "pricing:update",
            Delete = "pricing:delete";

        public static readonly string[] AllPermissions = { Read, Create, Access, Update, Delete };
    }
}
