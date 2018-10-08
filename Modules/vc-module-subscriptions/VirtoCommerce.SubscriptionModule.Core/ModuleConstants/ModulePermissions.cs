namespace VirtoCommerce.SubscriptionModule.Core.ModuleConstants
{
    public static class ModulePermissions
    {
        public const string Read = "subscription:read",
            Create = "subscription:create",
            Access = "subscription:access",
            Update = "subscription:update",
            Delete = "subscription:delete",
            PlanManage = "paymentplan:manage";

        public static readonly string[] AllPermissions = { Read, Create, Access, Update, Delete, PlanManage };
    }
}
