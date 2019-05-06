using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Data.ExportImport
{
    public sealed class PlatformExportSecurityEntries
    {
        public PlatformExportSecurityEntries()
        {
            Users = new List<ApplicationUser>();
            Roles = new List<Role>();
        }

        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<Role> Roles { get; set; }

    }
}
