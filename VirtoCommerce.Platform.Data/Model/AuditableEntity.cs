using System;
using System.ComponentModel.DataAnnotations;
using EntityFrameworkCore.Triggers;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Data.Model
{
    public abstract class AuditableEntity : IEntity, IAuditable
    {      
        [StringLength(128)]
        [Key]
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [StringLength(64)]
        public string CreatedBy { get; set; }
        [StringLength(64)]
        public string ModifiedBy { get; set; }
    }
}
