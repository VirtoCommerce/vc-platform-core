using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class PropertyDictionaryValueEntity : Entity
    {
        [StringLength(512)]
        public string Value { get; set; }

        [StringLength(64)]
        public string Locale { get; set; }

        #region Navigation Properties

        public string DictionaryItemId { get; set; }
        public virtual PropertyDictionaryItemEntity DictionaryItem { get; set; }

        #endregion

        public virtual PropertyDictionaryItemLocalizedValue ToModel(PropertyDictionaryItemLocalizedValue localizedValue)
        {
            if (localizedValue == null)
            {
                throw new ArgumentNullException(nameof(localizedValue));
            }
            localizedValue.LanguageCode = Locale;
            localizedValue.Value = Value;
            return localizedValue;
        }

        public virtual PropertyDictionaryValueEntity FromModel(PropertyDictionaryItemLocalizedValue localizedValue, PrimaryKeyResolvingMap pkMap)
        {
            if (localizedValue == null)
            {
                throw new ArgumentNullException(nameof(localizedValue));
            }
            Locale = localizedValue.LanguageCode;
            Value = localizedValue.Value;
            return this;
        }

        public virtual void Patch(PropertyDictionaryValueEntity target)
        {
            target.Locale = Locale;
            target.Value = Value;
        }
    }
}
