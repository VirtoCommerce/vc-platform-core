using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.StoreModule.Data.Model
{
    public class StoreLanguageEntity : Entity, IHasLanguageCode, ICloneable
    {
        [Required]
        [StringLength(32)]
        public string LanguageCode { get; set; }

        #region Navigation Properties

        public string StoreId { get; set; }
        public StoreEntity Store { get; set; }

        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as StoreLanguageEntity;

            if (Store != null)
            {
                result.Store = Store.Clone() as StoreEntity;
            }

            return result;
        }

        #endregion
    }
}
