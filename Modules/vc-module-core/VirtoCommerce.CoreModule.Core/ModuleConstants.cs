using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CoreModule.Core
{
    public class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string PackageTypeCreate = "core:packageType:create";
                public const string PackageTypeUpdate = "core:packageType:update";
                public const string PackageTypeDelete = "core:packageType:delete";
                public const string CurrencyCreate = "core:currency:create";
                public const string CurrencyUpdate = "core:currency:update";
                public const string CurrencyDelete = "core:currency:delete";

                public static string[] AllPermissions = new[] { PackageTypeCreate, PackageTypeUpdate, PackageTypeDelete, CurrencyCreate, CurrencyUpdate, CurrencyDelete };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor TaxTypes = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Core.General.TaxTypes",
                    GroupName = "Core|General",
                    ValueType = SettingValueType.ShortText,
                    IsDictionary = true,
                };

                public static SettingDescriptor WeightUnits = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Core.General.WeightUnits",
                    GroupName = "Core|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "gram",
                    IsDictionary = true,
                    AllowedValues = new string[] { "gram", "ounce", "pound" }
                };

                public static SettingDescriptor MeasureUnits = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Core.General.MeasureUnits",
                    GroupName = "Core|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "mm",
                    IsDictionary = true,
                    AllowedValues = new string[] { "m", "mm", "ft", "in" }
                };

                public static SettingDescriptor Languages = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Core.General.Languages",
                    GroupName = "Core|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "en-US",
                    IsDictionary = true,
                    AllowedValues = new string[] { "en-US", "fr-FR", "de-DE", "ja-JP" }
                };

                public static SettingDescriptor FixedRateShippingMethodGroundRate = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Core.FixedRateShippingMethod.Ground.Rate",
                    GroupName = "Core|General",
                    ValueType = SettingValueType.Decimal,
                    DefaultValue = 0.00m,
                };

                public static SettingDescriptor FixedRateShippingMethodAirRate = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Core.FixedRateShippingMethod.Air.Rate",
                    GroupName = "Core|General",
                    ValueType = SettingValueType.Decimal,
                    DefaultValue = 0.00m,
                };

                public static SettingDescriptor FixedTaxRateProviderRate = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Core.FixedTaxRateProvider.Rate",
                    GroupName = "Core|General",
                    ValueType = SettingValueType.Decimal,
                    DefaultValue = 0.00m,
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return TaxTypes;
                        yield return WeightUnits;
                        yield return MeasureUnits;
                        yield return Languages;
                        yield return FixedRateShippingMethodGroundRate;
                        yield return FixedRateShippingMethodAirRate;
                        yield return FixedTaxRateProviderRate;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings
            {
                get
                {
                    return General.AllSettings;
                }
            }
        }
    }


}
