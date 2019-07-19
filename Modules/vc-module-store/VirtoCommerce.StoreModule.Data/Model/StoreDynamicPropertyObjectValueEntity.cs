using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.StoreModule.Data.Model
{
    public class StoreDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual StoreEntity Store { get; set; }

        #region ICloneable members

        public override object Clone()
        {
            var result = base.Clone() as StoreDynamicPropertyObjectValueEntity;

            if (Store != null)
            {
                result.Store = Store.Clone() as StoreEntity;
            }

            return result;
        }

        #endregion
    }
}
