using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Data.ExportImport
{
    public sealed class PlatformExportEntries
    {
        public PlatformExportEntries()
        {
            Users = new List<ApplicationUser>();
            Settings = new List<ObjectSettingEntry>();
            DynamicPropertyDictionaryItems = new List<DynamicPropertyDictionaryItem>();
        }

        public bool IsNotEmpty => Users.Any() || Settings.Any();
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<ObjectSettingEntry> Settings { get; set; }
        public ICollection<DynamicPropertyDictionaryItem> DynamicPropertyDictionaryItems { get; set; }
        public ICollection<DynamicProperty> DynamicProperties { get; set; }

    }
}
