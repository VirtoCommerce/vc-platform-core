using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Security.Model
{
    public class RoleEntity : IdentityRole
    {
        public RoleEntity()
        {
            RolePermissions = new NullCollection<RolePermissionEntity>();
        }

        public string Description { get; set; }

        public virtual ObservableCollection<RolePermissionEntity> RolePermissions { get; set; }
    }
}
