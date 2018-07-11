using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Modularity;

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
                public static ModuleSetting TaxTypes = new ModuleSetting
                {
                    Name = "VirtoCommerce.Core.General.TaxTypes",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = "Default",
                    IsArray = true,
                };

                public static ModuleSetting WeightUnits = new ModuleSetting
                {
                    Name = "VirtoCommerce.Core.General.WeightUnits",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = "gram",
                    IsArray = true,
                    ArrayValues = new string[] { "gram", "ounce", "pound" }
                };

                public static ModuleSetting MeasureUnits = new ModuleSetting
                {
                    Name = "VirtoCommerce.Core.General.MeasureUnits",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = "mm",
                    IsArray = true,
                    ArrayValues = new string[] { "m", "mm", "ft", "in" }
                };

                public static ModuleSetting Languages = new ModuleSetting
                {
                    Name = "VirtoCommerce.Core.General.Languages",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = "en-US",
                    IsArray = true,
                    ArrayValues = new string[] { "en-US", "fr-FR", "de-DE", "ja-JP" }
                };

                public static ModuleSetting FixedRateShippingMethodGroundRate = new ModuleSetting
                {
                    Name = "VirtoCommerce.Core.FixedRateShippingMethod.Ground.Rate",
                    ValueType = ModuleSetting.TypeDecimal,
                    DefaultValue = "0.00",
                };

                public static ModuleSetting FixedRateShippingMethodAirRate = new ModuleSetting
                {
                    Name = "VirtoCommerce.Core.FixedRateShippingMethod.Air.Rate",
                    ValueType = ModuleSetting.TypeDecimal,
                    DefaultValue = "0.00",
                };

                public static ModuleSetting FixedTaxRateProviderRate = new ModuleSetting
                {
                    Name = "VirtoCommerce.Core.FixedTaxRateProvider.Rate",
                    ValueType = ModuleSetting.TypeDecimal,
                    DefaultValue = "0.00",
                };

                public static IEnumerable<ModuleSetting> AllSettings
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
        }
    }

    
}
