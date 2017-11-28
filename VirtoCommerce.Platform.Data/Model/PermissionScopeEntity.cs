using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Data.Model
{
    public class PermissionScopeEntity : Entity
    {
        [Required]
        [StringLength(1024)]
        public string Scope { get; set; }

        [StringLength(255)]
        public string Type { get; set; }

        [StringLength(1024)]
        public string Label { get; set; }

        #region Navigation properties
        public string RolePermissionId { get; set; }
        public RolePermissionEntity RolePermission { get; set; } 
        #endregion
    }
}
