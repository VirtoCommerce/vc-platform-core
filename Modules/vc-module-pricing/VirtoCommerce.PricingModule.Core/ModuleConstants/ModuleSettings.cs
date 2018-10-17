using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.PricingModule.Core.ModuleConstants
{
    public static class ModuleSettings
    {
        public static readonly SettingDescriptor ExportImportPageSize = new SettingDescriptor
        {
            Name = "Pricing.ExportImport.PageSize",
            GroupName = "Pricing|General",
            ValueType = SettingValueType.Integer,
            DefaultValue = 50
        };

        public static IEnumerable<SettingDescriptor> AllSettings
        {
            get
            {
                yield return ExportImportPageSize;
            }
        }
    }
}
