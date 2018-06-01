using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{
    public class ApplicationUser : IdentityUser, IEntity
    {
        /// <summary>
        /// Tenant id
        /// </summary>
        public virtual string StoreId { get; set; }
        public virtual string MemberId { get; set; }
        public virtual bool IsAdministrator { get; set; }
        public virtual string PhotoUrl { get; set; }
        public virtual string UserType { get; set; }
        public virtual string Password { get; set; }
        public virtual IList<Role> Roles { get; set; }
    }
}
