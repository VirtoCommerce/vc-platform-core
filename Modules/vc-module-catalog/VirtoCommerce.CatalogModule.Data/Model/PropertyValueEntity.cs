using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class PropertyValueEntity : AuditableEntity, IHasOuterId
    {
        [NotMapped]
        public string Alias { get; set; }

        [StringLength(64)]
        public string Name { get; set; }

        [Required]
        public int ValueType { get; set; }

        [StringLength(512)]
        public string ShortTextValue { get; set; }

        public string LongTextValue { get; set; }

        public decimal DecimalValue { get; set; }

        public int IntegerValue { get; set; }

        public bool BooleanValue { get; set; }

        public DateTime? DateTimeValue { get; set; }

        [StringLength(64)]
        public string Locale { get; set; }

        [StringLength(128)]
        public string OuterId { get; set; }

        #region Navigation Properties

        public string ItemId { get; set; }
        public virtual ItemEntity CatalogItem { get; set; }

        public string CatalogId { get; set; }
        public virtual CatalogEntity Catalog { get; set; }

        public string CategoryId { get; set; }
        public virtual CategoryEntity Category { get; set; }

        public string DictionaryItemId { get; set; }
        public virtual PropertyDictionaryItemEntity DictionaryItem { get; set; }

        #endregion

        public virtual IEnumerable<PropertyValue> ToModel(PropertyValue propValue)
        {
            if (propValue == null)
            {
                throw new ArgumentNullException(nameof(propValue));
            }

            propValue.Id = Id;
            propValue.CreatedBy = CreatedBy;
            propValue.CreatedDate = CreatedDate;
            propValue.ModifiedBy = ModifiedBy;
            propValue.ModifiedDate = ModifiedDate;
            propValue.OuterId = OuterId;

            propValue.LanguageCode = Locale;
            propValue.PropertyName = Name;
            propValue.ValueId = DictionaryItemId;
            propValue.ValueType = (PropertyValueType)ValueType;
            propValue.Value = DictionaryItem != null ? DictionaryItem.Alias : GetValue(propValue.ValueType);
            propValue.Alias = DictionaryItem?.Alias;
            //Need to expand all dictionary values
            if (DictionaryItem != null && !DictionaryItem.DictionaryItemValues.IsNullOrEmpty())
            {
                foreach (var dictItemValue in DictionaryItem.DictionaryItemValues)
                {
                    var dictPropValue = propValue.Clone() as PropertyValue;
                    dictPropValue.Alias = DictionaryItem.Alias;
                    dictPropValue.ValueId = DictionaryItem.Id;
                    dictPropValue.LanguageCode = dictItemValue.Locale;
                    dictPropValue.Value = dictItemValue.Value;
                    yield return dictPropValue;

                }
            }
            else
            {
                yield return propValue;
            }
        }

        public virtual IEnumerable<PropertyValueEntity> FromModels(IEnumerable<PropertyValue> propValues, PrimaryKeyResolvingMap pkMap)
        {
            if (propValues == null)
            {
                throw new ArgumentNullException(nameof(propValues));
            }

            var groupedValues = propValues.Where(x => !x.IsInherited && (!string.IsNullOrEmpty(x.ValueId) || !string.IsNullOrEmpty(x.Value?.ToString())))
                                           .Select(x => AbstractTypeFactory<PropertyValueEntity>.TryCreateInstance().FromModel(x, pkMap))
                                           .GroupBy(x => x.DictionaryItemId);

            var result = new List<PropertyValueEntity>();
            foreach (var group in groupedValues)
            {
                if (group.Key == null)
                {
                    result.AddRange(group);
                }
                else
                {
                    result.Add(group.FirstOrDefault());
                }
            }

            return result;
        }

        public virtual PropertyValueEntity FromModel(PropertyValue propValue, PrimaryKeyResolvingMap pkMap)
        {
            if (propValue == null)
                throw new ArgumentNullException(nameof(propValue));

            pkMap.AddPair(propValue, this);

            Id = propValue.Id;
            CreatedBy = propValue.CreatedBy;
            CreatedDate = propValue.CreatedDate;
            ModifiedBy = propValue.ModifiedBy;
            ModifiedDate = propValue.ModifiedDate;
            OuterId = propValue.OuterId;

            Name = propValue.PropertyName;
            ValueType = (int)propValue.ValueType;
            DictionaryItemId = propValue.ValueId;
            //Required for manual reference
            Alias = propValue.Alias;
            //Store alias as value for dictionary properties values
            SetValue(propValue.ValueType, !string.IsNullOrEmpty(DictionaryItemId) ? propValue.Alias : propValue.Value);
            Locale = !string.IsNullOrEmpty(DictionaryItemId) ? null : propValue.LanguageCode;

            return this;
        }

        public virtual void Patch(PropertyValueEntity target)
        {
            target.BooleanValue = BooleanValue;
            target.DateTimeValue = DateTimeValue;
            target.DecimalValue = DecimalValue;
            target.IntegerValue = IntegerValue;
            target.DictionaryItemId = DictionaryItemId;
            target.Locale = Locale;
            target.LongTextValue = LongTextValue;
            target.Name = Name;
            target.ShortTextValue = ShortTextValue;
            target.ValueType = ValueType;
        }

        protected virtual object GetValue(PropertyValueType valueType)
        {
            switch (ValueType)
            {
                case (int)PropertyValueType.Boolean:
                    return BooleanValue;
                case (int)PropertyValueType.DateTime:
                    return DateTimeValue;
                case (int)PropertyValueType.Number:
                    return DecimalValue;
                case (int)PropertyValueType.LongText:
                    return LongTextValue;
                case (int)PropertyValueType.Integer:
                    return IntegerValue;
                default:
                    return ShortTextValue;
            }
        }

        protected virtual void SetValue(PropertyValueType valueType, object value)
        {
            switch (valueType)
            {
                case PropertyValueType.LongText:
                    LongTextValue = Convert.ToString(value);
                    break;
                case PropertyValueType.ShortText:
                    ShortTextValue = Convert.ToString(value);
                    break;
                case PropertyValueType.Number:
                    DecimalValue = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                    break;
                case PropertyValueType.DateTime:
                    DateTimeValue = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                    break;
                case PropertyValueType.Boolean:
                    BooleanValue = Convert.ToBoolean(value);
                    break;
                case PropertyValueType.Integer:
                    IntegerValue = Convert.ToInt32(value);
                    break;
                case PropertyValueType.GeoPoint:
                    ShortTextValue = Convert.ToString(value);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
