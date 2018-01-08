using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
namespace VirtoCommerce.Platform.Core.Security
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
        public IList<Permission> Permissions { get; set; }
    }
}
