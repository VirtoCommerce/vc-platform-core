using System;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.StoreModule.Data.Model
{
    public class StoreDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity, ICloneable
    {
        public virtual StoreEntity Store { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as StoreDynamicPropertyObjectValueEntity;
            return result;
        }

        #endregion
    }
}
