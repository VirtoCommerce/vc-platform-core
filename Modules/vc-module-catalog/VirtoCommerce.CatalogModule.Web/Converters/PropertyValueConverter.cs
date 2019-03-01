using moduleModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class PropertyValueConverter
    {
        public static webModel.PropertyValue ToWebModel(this moduleModel.PropertyValue propValue)
        {
            var retVal = new webModel.PropertyValue
            {
                Id = propValue.Id,
                LanguageCode = propValue.LanguageCode,
                PropertyId = propValue.PropertyId,
                PropertyName = propValue.PropertyName,
                ValueId = propValue.ValueId,
                ValueType = propValue.ValueType,
                Alias = propValue.Alias,
                IsInherited = propValue.IsInherited
            };


            if (propValue.Property != null)
            {
                retVal.PropertyId = propValue.Property.Id;
                retVal.PropertyMultivalue = propValue.Property.Multivalue;
            }
            retVal.Value = propValue.Value;

            return retVal;
        }

        public static moduleModel.PropertyValue ToCoreModel(this webModel.PropertyValue propValue)
        {
            var retVal = new moduleModel.PropertyValue
            {
                Id = propValue.Id,
                LanguageCode = propValue.LanguageCode,
                PropertyId = propValue.PropertyId,
                PropertyName = propValue.PropertyName,
                ValueId = propValue.ValueId,
                ValueType = propValue.ValueType,
                Alias = propValue.Alias,
                IsInherited = propValue.IsInherited,
                Value = propValue.Value
            };
            retVal.ValueType = (moduleModel.PropertyValueType)(int)propValue.ValueType;
            retVal.Property = new moduleModel.Property();
            return retVal;
        }
    }
}
