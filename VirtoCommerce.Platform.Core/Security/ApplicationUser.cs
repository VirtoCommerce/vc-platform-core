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

        /// <summary>
        /// Indicates that the password for this user is expired and must be changed.
        /// </summary>
        public virtual bool PasswordExpired { get; set; }

        //This override methods required to correct working of the Identity DB context updates.
        //DbContext throws out an "object of an already tracked object" when attempting to update a user on an object that wasn't retrieved from the DbContext
        public override bool Equals(object obj)
        {
            if (!(obj is ApplicationUser otherUser))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (string.IsNullOrEmpty(Id))
                return false;

            return otherUser.Id == Id;
        }

        public override int GetHashCode()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                return Id.GetHashCode();
            }
            return base.GetHashCode();
        }
    }
}
