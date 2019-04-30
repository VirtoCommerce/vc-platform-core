using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
        public IList<Permission> Permissions { get; set; }

        //This override methods required to correct working of the Identity DB context updates.
        //DbContext throws out an "object of an already tracked object" when attempting to update a user on an object that wasn't retrieved from the DbContext
        public override bool Equals(object obj)
        {
            if (!(obj is Role otherUser))
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

        public virtual void Patch(Role target)
        {
            target.Name = Name;
            target.NormalizedName = NormalizedName;
            target.ConcurrencyStamp = ConcurrencyStamp;
            target.Description = Description;

            if (!Permissions.IsNullCollection())
            {
                Permissions.Patch(target.Permissions, (sourcePermission, targetPermission) => sourcePermission.Patch(targetPermission));
            }
        }
    }
}
