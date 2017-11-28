using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{
    public class RoleSearchResponse : ValueObject
    {
        public Role[] Roles { get; set; }
        public int TotalCount { get; set; }
    }
}
