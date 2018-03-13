using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{

    public class PropertyValueEntity : AuditableEntity
    {
        [StringLength(64)]
        public string Alias { get; set; }

        [StringLength(64)]
        public string Name { get; set; }

        [StringLength(128)]
        public string KeyValue { get; set; }

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


        #region Navigation Properties
        public string ItemId { get; set; }
        public virtual ItemEntity CatalogItem { get; set; }

        public string CatalogId { get; set; }
        public virtual CatalogEntity Catalog { get; set; }

        public string CategoryId { get; set; }
        public virtual CategoryEntity Category { get; set; }
        #endregion

        public virtual PropertyValue ToModel(PropertyValue propValue)
        {
            if (propValue == null)
                throw new ArgumentNullException(nameof(propValue));

            propValue.Id = Id;
            propValue.CreatedBy = CreatedBy;
            propValue.CreatedDate = CreatedDate;
            propValue.ModifiedBy = ModifiedBy;
            propValue.ModifiedDate = ModifiedDate;

            propValue.Alias = Alias;
            propValue.LanguageCode = Locale;
            propValue.PropertyName = Name;
            propValue.ValueId = KeyValue;
            propValue.ValueType = (PropertyValueType)ValueType;
            propValue.Value = GetValue(propValue.ValueType);

            return propValue;
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

            Alias = propValue.Alias;
            Locale = propValue.LanguageCode;
            Name = propValue.PropertyName;
            ValueType = (int)propValue.ValueType;
            KeyValue = propValue.ValueId;
            SetValue(propValue.ValueType, propValue.Value);

            return this;
        }

        public virtual void Patch(PropertyValueEntity target)
        {
            target.Alias = Alias;
            target.BooleanValue = BooleanValue;
            target.DateTimeValue = DateTimeValue;
            target.DecimalValue = DecimalValue;
            target.IntegerValue = IntegerValue;
            target.KeyValue = KeyValue;
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
