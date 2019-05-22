using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.OrdersModule.Core
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
                public const string ReadPrices = "order:read_prices";

                public static string[] AllPermissions = new[] { Read, Create, Update, Access, Delete, ReadPrices };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor OrderStatus = new SettingDescriptor
                {
                    Name = "Order.Status",
                    ValueType = SettingValueType.ShortText,
                    GroupName = "Orders|General",
                    IsDictionary = true,
                    DefaultValue = "New",
                    AllowedValues = new[] { "New", "Not payed", "Pending", "Processing", "Ready to send", "Cancelled", "Partially sent", "Completed" }
                };

                public static SettingDescriptor ShipmentStatus = new SettingDescriptor
                {
                    Name = "Shipment.Status",
                    ValueType = SettingValueType.ShortText,
                    GroupName = "Orders|General",
                    IsDictionary = true,
                    DefaultValue = "New",
                    AllowedValues = new[] { "New", "PickPack", "Cancelled", "ReadyToSend", "Sent" }
                };

                public static SettingDescriptor PaymentInStatus = new SettingDescriptor
                {
                    Name = "PaymentIn.Status",
                    ValueType = SettingValueType.ShortText,
                    GroupName = "Orders|General",
                    IsDictionary = true,
                    DefaultValue = "New",
                    AllowedValues = new[] { "New", "Pending", "Authorized", "Paid", "PartiallyRefunded", "Refunded", "Voided", "Custom", "Cancelled" }
                };

                public static SettingDescriptor OrderCustomerOrderNewNumberTemplate = new SettingDescriptor
                {
                    Name = "Order.CustomerOrderNewNumberTemplate",
                    ValueType = SettingValueType.ShortText,
                    GroupName = "Orders|General",
                    DefaultValue = "CO{0:yyMMdd}-{1:D5}"
                };
                
                public static SettingDescriptor OrderShipmentNewNumberTemplate = new SettingDescriptor
                {
                    Name = "Order.ShipmentNewNumberTemplate",
                    ValueType = SettingValueType.ShortText,
                    GroupName = "Orders|General",
                    DefaultValue = "SH{0:yyMMdd}-{1:D5}"
                };

                public static SettingDescriptor OrderPaymentInNewNumberTemplate = new SettingDescriptor
                {
                    Name = "Order.PaymentInNewNumberTemplate",
                    ValueType = SettingValueType.ShortText,
                    GroupName = "Orders|General",
                    DefaultValue = "PI{0:yyMMdd}-{1:D5}"
                };

                public static SettingDescriptor SendOrderNotifications = new SettingDescriptor
                {
                    Name = "Order.SendOrderNotifications",
                    GroupName = "Store|General",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = true,
                };

                public static SettingDescriptor OrderAdjustInventory = new SettingDescriptor
                {
                    Name = "Order.AdjustInventory",
                    GroupName = "Store|General",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = true,
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return OrderStatus;
                        yield return ShipmentStatus;
                        yield return PaymentInStatus;
                        yield return OrderCustomerOrderNewNumberTemplate;
                        yield return OrderShipmentNewNumberTemplate;
                        yield return OrderPaymentInNewNumberTemplate;
                        yield return SendOrderNotifications;
                        yield return OrderAdjustInventory;
                    }
                }
            }
        }
    }
}
