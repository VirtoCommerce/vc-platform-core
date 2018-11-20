using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.SubscriptionModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "subscription:read";
                public const string Create = "subscription:create";
                public const string Access = "subscription:access";
                public const string Update = "subscription:update";
                public const string Delete = "subscription:delete";
                public const string PlanManage = "paymentplan:manage";

                public static readonly string[] AllPermissions = { Read, Create, Access, Update, Delete, PlanManage };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static readonly SettingDescriptor EnableSubscriptions = new SettingDescriptor
                {
                    Name = "Subscription.EnableSubscriptions",
                    GroupName = "Subscriptions|General",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = true
                };

                public static readonly SettingDescriptor StatusValues = new SettingDescriptor
                {
                    Name = "Subscription.Status",
                    GroupName = "Subscriptions|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "Active",
                    IsDictionary = true,
                    AllowedValues = new[] { "Trialling", "Active", "PastDue", "Unpaid", "Cancelled" }
                };

                public static readonly SettingDescriptor NewNumberTemplate = new SettingDescriptor
                {
                    Name = "Subscription.SubscriptionNewNumberTemplate",
                    GroupName = "Subscriptions|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "SU{0:yyMMdd}-{1:D5}"
                };

                public static readonly SettingDescriptor EnableSubscriptionProcessJob = new SettingDescriptor
                {
                    Name = "Subscription.EnableSubscriptionProcessJob",
                    GroupName = "Subscriptions|General",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = true,
                    RestartRequired = true
                };

                public static readonly SettingDescriptor EnableSubscriptionOrdersCreateJob = new SettingDescriptor
                {
                    Name = "Subscription.EnableSubscriptionOrdersCreateJob",
                    GroupName = "Subscriptions|General",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = true,
                    RestartRequired = true
                };

                public static readonly SettingDescriptor CronExpression = new SettingDescriptor
                {
                    Name = "Subscription.CronExpression",
                    GroupName = "Subscriptions|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "0 */1 * * *",
                    RestartRequired = true
                };

                public static readonly SettingDescriptor CronExpressionOrdersJob = new SettingDescriptor
                {
                    Name = "Subscription.CronExpressionOrdersJob",
                    GroupName = "Subscriptions|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "0 0 */1 * *",
                    RestartRequired = true
                };

                public static readonly SettingDescriptor PastDueDelay = new SettingDescriptor
                {
                    Name = "Subscription.PastDue.Delay",
                    GroupName = "Subscriptions|General",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = 7
                };

                public static readonly SettingDescriptor ExportImportDescription = new SettingDescriptor
                {
                    Name = "Subscription.ExportImport.Description",
                    GroupName = "Subscriptions|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "Export/Import of subscriptions"
                };

                public static IEnumerable<SettingDescriptor> StoreLevelSettings
                {
                    get
                    {
                        yield return EnableSubscriptions;
                    }
                }

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return EnableSubscriptions;
                        yield return StatusValues;
                        yield return NewNumberTemplate;
                        yield return EnableSubscriptionProcessJob;
                        yield return EnableSubscriptionOrdersCreateJob;
                        yield return CronExpression;
                        yield return CronExpressionOrdersJob;
                        yield return PastDueDelay;
                        yield return ExportImportDescription;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> StoreLevelSettings => General.StoreLevelSettings;
            public static IEnumerable<SettingDescriptor> AllSettings => General.AllSettings;
        }
    }
}
