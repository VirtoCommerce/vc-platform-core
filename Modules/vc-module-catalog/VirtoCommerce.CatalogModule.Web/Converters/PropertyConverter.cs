using System.Collections.Generic;
using System.Linq;
using coreModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class PropertyConverter
    {
        public static webModel.Property ToWebModel(this coreModel.Property property)
        {
            var retVal = new webModel.Property
            {
                Id = property.Id,
                Name = property.Name,
                Required = property.Required,
                Type = property.Type,
                Multivalue = property.Multivalue,
                CatalogId = property.CatalogId,
                CategoryId = property.CategoryId,
                Dictionary = property.Dictionary,
                ValueType = property.ValueType
            };
            retVal.Type = property.Type;
            retVal.Multilanguage = property.Multilanguage;
            retVal.IsInherited = property.IsInherited;

            retVal.ValueType = property.ValueType;
            retVal.Type = property.Type;

            if (property.Attributes != null)
            {
                retVal.Attributes = property.Attributes.Select(x => x.ToWebModel()).ToList();
            }
            retVal.DisplayNames = property.DisplayNames;

            retVal.ValidationRule = property.ValidationRules.FirstOrDefault()?.ToWebModel();

            return retVal;
        }

        public static coreModel.Property ToCoreModel(this webModel.Property property)
        {
            var retVal = new coreModel.Property
            {
                Id = property.Id,
                Name = property.Name,
                Required = property.Required,
                Type = (coreModel.PropertyType)(int)property.Type,
                Multivalue = property.Multivalue,
                CatalogId = property.CatalogId,
                CategoryId = property.CategoryId,
                Dictionary = property.Dictionary,
                ValueType = (coreModel.PropertyValueType)(int)property.ValueType,
                DisplayNames = property.DisplayNames
            };

            if (property.Attributes != null)
            {
                retVal.Attributes = property.Attributes.Select(x => x.ToCoreModel()).ToList();
            }

            if (property.ValidationRule != null)
            {
                retVal.ValidationRules = new List<coreModel.PropertyValidationRule>() { property.ValidationRule.ToCoreModel() };
            }
            else retVal.ValidationRules = new List<coreModel.PropertyValidationRule>();

            return retVal;
        }
    }
}
