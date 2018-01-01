using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Security.Model
{
    public class RoleAssignmentEntity : AuditableEntity
    {

        private string _roleName = null;
        [NotMapped]
        public string RoleName
        {
            get
            {
                if (_roleName == null)
                {
                    _roleName = Role?.Name;
                }
                return _roleName;
            }
            set
            {
                _roleName = value;
            }
        }

        public string RoleId { get; set; }
        public virtual RoleEntity Role { get; set; }
        public string AccountId { get; set; }
        public virtual ApplicationUserEntity Account { get; set; }

    }
}
