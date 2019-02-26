using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CatalogModule.Core2.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data2.Model
{
    public class PropertyDictionaryValueEntity : Entity
    {
        [StringLength(64)]
        public string Alias { get; set; }

        [StringLength(64)]
        public string Name { get; set; }

        [StringLength(512)]
        public string Value { get; set; }

        [StringLength(64)]
        public string Locale { get; set; }

        #region Navigation Properties
        public string PropertyId { get; set; }
        public virtual PropertyEntity Property { get; set; }
        #endregion

        public virtual PropertyDictionaryValue ToModel(PropertyDictionaryValue dictValue)
        {
            if (dictValue == null)
                throw new ArgumentNullException(nameof(dictValue));

            dictValue.Id = Id;
            dictValue.Alias = Alias;
            dictValue.LanguageCode = Locale;
            dictValue.PropertyId = PropertyId;
            dictValue.Value = Value;

            return dictValue;
        }

        public virtual PropertyDictionaryValueEntity FromModel(PropertyDictionaryValue dictValue, PrimaryKeyResolvingMap pkMap)
        {
            if (dictValue == null)
                throw new ArgumentNullException(nameof(dictValue));

            pkMap.AddPair(dictValue, this);

            Id = dictValue.Id;
            Alias = dictValue.Alias;
            Value = dictValue.Value;
            PropertyId = dictValue.PropertyId;
            Locale = dictValue.LanguageCode;

            return this;
        }

        public virtual void Patch(PropertyDictionaryValueEntity target)
        {
            target.Value = Value;
            target.Alias = Alias;
            target.Locale = Locale;
        }
    }
}
