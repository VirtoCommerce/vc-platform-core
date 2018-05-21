using System;
using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Platform.Core.Common
{
    public abstract class AuditableEntity : Entity, IAuditable
    {
        #region IAuditable Members
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [StringLength(64)]
        public string CreatedBy { get; set; }
        [StringLength(64)]
        public string ModifiedBy { get; set; }
        #endregion
    }
}
