using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.StoreModule.Data.Model
{
    public class StoreTrustedGroupEntity : Entity
    {
        [Required]
        [StringLength(128)]
        public string GroupName { get; set; }

        #region Navigation Properties

        public string StoreId { get; set; }
        public StoreEntity Store { get; set; }

        #endregion
    }
}
