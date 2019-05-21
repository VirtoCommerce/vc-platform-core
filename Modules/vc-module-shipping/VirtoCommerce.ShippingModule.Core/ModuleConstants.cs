using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ShippingModule.Core
{
    public class ModuleConstants
    {
        public static class Settings
        {
            public static class General
            {
                public static IEnumerable<SettingDescriptor> AllSettings => Enumerable.Empty<SettingDescriptor>();
            }

            public static class FixedRateShippingMethod
            {
                public static SettingDescriptor GroundRate = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Shipping.FixedRateShippingMethod.Ground.Rate",
                    GroupName = "Shipping|General",
                    ValueType = SettingValueType.Decimal,
                    DefaultValue = 0.00m,
                };

                public static SettingDescriptor AirRate = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Shipping.FixedRateShippingMethod.Air.Rate",
                    GroupName = "Shipping|General",
                    ValueType = SettingValueType.Decimal,
                    DefaultValue = 0.00m,
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return GroundRate;
                        yield return AirRate;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings => General.AllSettings.Concat(FixedRateShippingMethod.AllSettings);
        }
    }
}
