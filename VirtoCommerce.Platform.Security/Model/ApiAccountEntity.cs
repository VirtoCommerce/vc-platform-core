using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.Platform.Security.Model
{
    public class ApiAccountEntity : AuditableEntity
    {
        [StringLength(128)]
        public string Name { get; set; }
        [StringLength(64)]
        public string ApiAccountType { get; set; }
        public string AccountId { get; set; }

        [StringLength(128)]
        [Required]
        public string AppId { get; set; }
        public string SecretKey { get; set; }
        public bool IsActive { get; set; }

        public virtual ApplicationUserEntity Account { get; set; }
    }
}
