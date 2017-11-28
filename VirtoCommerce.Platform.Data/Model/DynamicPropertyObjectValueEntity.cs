using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.Platform.Data.Model
{
    public class DynamicPropertyObjectValueEntity : AuditableEntity
    {
        [StringLength(256)]
        public string ObjectType { get; set; }

        [StringLength(128)]
        public string ObjectId { get; set; }

        [StringLength(64)]
        public string Locale { get; set; }

        [Required]
        [StringLength(64)]
        public string ValueType { get; set; }

        [StringLength(512)]
        public string ShortTextValue { get; set; }
        public string LongTextValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public int? IntegerValue { get; set; }
        public bool? BooleanValue { get; set; }
        public DateTime? DateTimeValue { get; set; }

        public string PropertyId { get; set; }
        public virtual DynamicPropertyEntity Property { get; set; }

        public string DictionaryItemId { get; set; }
        public virtual DynamicPropertyDictionaryItemEntity DictionaryItem { get; set; }

        public virtual DynamicPropertyObjectValue ToModel(DynamicPropertyObjectValue propValue)
        {
            if (propValue == null)
                throw new ArgumentNullException(nameof(propValue));

            propValue.Locale = Locale;
            propValue.ObjectId = ObjectId;
            propValue.ObjectType = ObjectType;
            propValue.ValueType = EnumUtility.SafeParse(ValueType, DynamicPropertyValueType.LongText);

            if (DictionaryItem != null)
            {
                propValue.Value = DictionaryItem.ToModel(AbstractTypeFactory<DynamicPropertyDictionaryItem>.TryCreateInstance());
            }
            else
            {
                propValue.Value = GetValue(propValue.ValueType);
            }
            return propValue;
        }

        public virtual DynamicPropertyObjectValueEntity FromModel(DynamicPropertyObjectValue propValue)
        {
            if (propValue == null)
                throw new ArgumentNullException(nameof(propValue));

            Locale = propValue.Locale;
            ObjectId = propValue.ObjectId;
            ObjectType = propValue.ObjectType;
            ValueType = propValue.ValueType.ToString();
            DictionaryItemId = propValue.ValueId;
            SetValue(propValue.ValueType, propValue.Value);

            return this;
        }

        public virtual void Patch(DynamicPropertyObjectValueEntity target)
        {
            target.Locale = Locale;
            target.LongTextValue = LongTextValue;
            target.BooleanValue = BooleanValue;
            target.DateTimeValue = DateTimeValue;
            target.DecimalValue = DecimalValue;
            target.DictionaryItemId = DictionaryItemId;
            target.IntegerValue = IntegerValue;
            target.ShortTextValue = ShortTextValue;            
        }              

        public virtual object GetValue(DynamicPropertyValueType valueType)
        {
            if (DictionaryItemId != null)
                return DictionaryItemId;

            switch (valueType)
            {
                case DynamicPropertyValueType.Boolean:
                    return BooleanValue;
                case DynamicPropertyValueType.DateTime:
                    return DateTimeValue;
                case DynamicPropertyValueType.Decimal:
                    return DecimalValue;
                case DynamicPropertyValueType.Integer:
                    return IntegerValue;
                case DynamicPropertyValueType.ShortText:
                    return ShortTextValue;
                default:
                    return LongTextValue;
            }
        }

        public virtual void SetValue(DynamicPropertyValueType valueType, object value)
        {
            switch (valueType)
            {
                case DynamicPropertyValueType.ShortText:
                    ShortTextValue = Convert.ToString(value);
                    break;
                case DynamicPropertyValueType.Decimal:
                    DecimalValue = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                    break;
                case DynamicPropertyValueType.DateTime:
                    DateTimeValue = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                    break;
                case DynamicPropertyValueType.Boolean:
                    BooleanValue = Convert.ToBoolean(value);
                    break;
                case DynamicPropertyValueType.Integer:
                    IntegerValue = Convert.ToInt32(value);
                    break;
                default:
                    LongTextValue = Convert.ToString(value);
                    break;
            }
        }
    }
}
