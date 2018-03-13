using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
	public class Property : AuditableEntity, IInheritable, ICloneable
    {
		public string CatalogId { get; set; }
		public Catalog Catalog { get; set; }
		public string CategoryId { get; set; }
		public Category Category { get; set; }
		public string Name { get; set; }
		public bool Required { get; set; }
		public bool Dictionary { get; set; }
		public bool Multivalue { get; set; }
		public bool Multilanguage { get; set; }
		public PropertyValueType ValueType { get; set; }
		public PropertyType Type { get; set; }
		public IList<PropertyAttribute> Attributes { get; set; }
		public IList<PropertyDictionaryValue> DictionaryValues { get; set; }
		public IList<PropertyDisplayName> DisplayNames { get; set; }
        public IList<PropertyValidationRule> ValidationRules { get; set; }
        public IList<PropertyValue> Values { get; set; }

        public virtual bool IsSame(Property other, params PropertyType[] additionalTypes)
        {
            if(other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            var result = Name.EqualsInvariant(other.Name) && ValueType == other.ValueType && (Type == other.Type || (additionalTypes?.Any(x=> x == other.Type) ?? false));
            return result;
        }
        /// <summary>
        /// Because we not have a direct link between prop values and properties we should
        /// find property value meta information by comparing key properties like name and value type.
        /// </summary>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public bool IsSuitableForValue(PropertyValue propValue)
        {
            return string.Equals(Name, propValue.PropertyName, StringComparison.InvariantCultureIgnoreCase) && ValueType == propValue.ValueType;
        }


        public virtual void ActualizeValues()
        {
            var dictLocalizedValues = new List<PropertyValue>();
            if (Values != null)
            {
                foreach (var propValue in Values)
                {
                    if (Dictionary && DictionaryValues != null)
                    {
                        /// Actualize  dictionary property value from property meta-information instead stored
                        if (propValue.ValueId != null)
                        {
                            var dictValue = DictionaryValues.FirstOrDefault(x => x.Id == propValue.ValueId);
                            if (dictValue != null)
                            {
                                propValue.Value = dictValue.Value;
                            }
                        }
                        if (Multilanguage)
                        {
                            foreach (var dictValue in DictionaryValues.Where(x => x.Alias.EqualsInvariant(propValue.Alias)))
                            {
                                var langDictPropValue = propValue.Clone() as PropertyValue;
                                langDictPropValue.Id = null;
                                langDictPropValue.LanguageCode = dictValue.LanguageCode;
                                langDictPropValue.Value = dictValue.Value;
                                dictLocalizedValues.Add(langDictPropValue);
                            }
                        }
                    }
                }
                foreach (var localizedDictValue in dictLocalizedValues)
                {
                    if (!Values.Any(x => x.Alias.EqualsInvariant(localizedDictValue.Alias) && x.LanguageCode.EqualsInvariant(localizedDictValue.LanguageCode)))
                    {
                        Values.Add(localizedDictValue);
                    }
                }
            }        
        }
      
        #region IInheritable Members
        public bool IsInherited { get; private set; }
        public virtual void TryInheritFrom(IEntity parent)
        {
            if (parent is Property parentProperty)
            {
                IsInherited = true;
                Id = parentProperty.Id;
                CreatedBy = parentProperty.CreatedBy;
                ModifiedBy = parentProperty.ModifiedBy;
                CreatedDate = parentProperty.CreatedDate;
                ModifiedDate = parentProperty.ModifiedDate;
                Required = parentProperty.Required;
                Dictionary = parentProperty.Dictionary;
                Multivalue = parentProperty.Multivalue;
                Multilanguage = parentProperty.Multilanguage;
                ValueType = parentProperty.ValueType;
                Type = parentProperty.Type;
                Attributes = parentProperty.Attributes;
                DictionaryValues = parentProperty.DictionaryValues;
                DisplayNames = parentProperty.DisplayNames;
                ValidationRules = parentProperty.ValidationRules;

                if (Values.IsNullOrEmpty() && !parentProperty.Values.IsNullOrEmpty())
                {
                    Values = new List<PropertyValue>();
                    foreach (var parentPropValue in parentProperty.Values)
                    {
                        var propValue = AbstractTypeFactory<PropertyValue>.TryCreateInstance();
                        propValue.TryInheritFrom(parentPropValue);
                        Values.Add(propValue);
                    }
                }

            }
        }
        #endregion

        #region ICloneable members
        public object Clone()
        {
            return base.MemberwiseClone();
        }
        #endregion
    }
}
