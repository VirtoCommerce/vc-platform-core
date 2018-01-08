using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{
    public class ApplicationUser: IdentityUser, IEntity
    {     
        /// <summary>
        /// Tenant id
        /// </summary>
        public virtual string StoreId { get; set; }
        public virtual string MemberId { get; set; }

        public IList<Role> Roles { get; set; }
    }
}
