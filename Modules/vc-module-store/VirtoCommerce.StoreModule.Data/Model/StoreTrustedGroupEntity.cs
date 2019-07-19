using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.StoreModule.Data.Model
{
    public class StoreTrustedGroupEntity : Entity, ICloneable
    {
        [Required]
        [StringLength(128)]
        public string GroupName { get; set; }

        #region Navigation Properties

        public string StoreId { get; set; }
        public StoreEntity Store { get; set; }

        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as StoreTrustedGroupEntity;

            if (Store != null)
            {
                result.Store = Store.Clone() as StoreEntity;
            }

            return result;
        }

        #endregion
    }
}
