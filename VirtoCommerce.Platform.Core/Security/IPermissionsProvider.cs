using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Platform.Core.Security
{
    public interface IPermissionsProvider
    {
        void RegisterPermissions(Permission[] permissions);
        IEnumerable<Permission> GetAllPermissions();
    }
}
