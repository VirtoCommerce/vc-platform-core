using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.TaxModule.Core
{
    public class ModuleConstants
    {
        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor TaxTypes = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Core.General.TaxTypes",
                    GroupName = "Tax|General",
                    ValueType = SettingValueType.ShortText,
                    IsDictionary = true,
                };


                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return TaxTypes;
                    }
                }
            }

            public static class FixedTaxProviderSettings
            {
                public static SettingDescriptor FixedTaxRateProviderRate = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Core.FixedTaxRateProvider.Rate",
                    GroupName = "Tax|FixedTaxProvider",
                    ValueType = SettingValueType.Decimal,
                    DefaultValue = 0.00m,
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return FixedTaxRateProviderRate;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings
            {
                get
                {
                    return General.AllSettings.Concat(FixedTaxProviderSettings.AllSettings);
                }
            }
        }
    }


}
