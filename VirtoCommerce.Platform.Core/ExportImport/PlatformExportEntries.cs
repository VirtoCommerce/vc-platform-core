using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Core.ExportImport
{
    public sealed class PlatformExportEntries
    {
        public PlatformExportEntries()
        {
            Users = new List<ApplicationUser>();
            Settings = new List<SettingEntry>();
            DynamicPropertyDictionaryItems = new List<DynamicPropertyDictionaryItem>();
        }
        public bool IsNotEmpty => Users.Any() || Settings.Any();
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<SettingEntry> Settings { get; set; }
        public ICollection<DynamicPropertyDictionaryItem> DynamicPropertyDictionaryItems { get; set; }
        public ICollection<DynamicProperty> DynamicProperties { get; set; }

    }
}
