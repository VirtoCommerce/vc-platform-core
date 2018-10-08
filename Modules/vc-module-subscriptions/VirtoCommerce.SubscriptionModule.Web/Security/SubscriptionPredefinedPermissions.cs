using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtoCommerce.SubscriptionModule.Web.Security
{
    public static class SubscriptionPredefinedPermissions
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
