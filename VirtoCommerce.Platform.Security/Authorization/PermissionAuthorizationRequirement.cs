using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Security.Authorization
{
    public class PermissionAuthorizationRequirement : IAuthorizationRequirement
    {
       public Permission Permission { get; set; }
    }
}
