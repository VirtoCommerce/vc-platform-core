using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Model;

namespace Module1.Data
{
    public class SettingEntity2 : SettingEntity
    {
        public string NewField { get; set; }

        public override SettingEntry ToModel(SettingEntry settingEntry)
        {
            var result = base.ToModel(settingEntry);
            if (result is SettingEntry2 settingEntry2)
            {
                settingEntry2.NewField = NewField;
            }
            return result;
        }

        public override SettingEntity FromModel(SettingEntry settingEntry)
        {
            var result = base.FromModel(settingEntry);
            if (result is SettingEntity2 entity2)
            {
                NewField = entity2.NewField;
            }
            return result;
        }
    }
}
