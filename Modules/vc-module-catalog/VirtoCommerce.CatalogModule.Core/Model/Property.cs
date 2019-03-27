using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class Property : AuditableEntity, IInheritable, ICloneable
    {
        public string CatalogId { get; set; }
        [JsonIgnore]
        public Catalog Catalog { get; set; }
        public string CategoryId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public bool Dictionary { get; set; }
        public bool Multivalue { get; set; }
        public bool Multilanguage { get; set; }
        public bool IsManageable { get; set; }
        public bool IsReadOnly { get; set; }
        public PropertyValueType ValueType { get; set; }
        public PropertyType Type { get; set; }
        public IList<PropertyAttribute> Attributes { get; set; }
        public IList<PropertyDisplayName> DisplayNames { get; set; }
        public IList<PropertyValidationRule> ValidationRules { get; set; }
        public IList<PropertyValue> Values { get; set; }

        public virtual bool IsSame(Property other, params PropertyType[] additionalTypes)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            var result = Name.EqualsInvariant(other.Name) && ValueType == other.ValueType && (Type == other.Type || (additionalTypes?.Any(x => x == other.Type) ?? false));
            return result;
        }
        /// <summary>
        /// Because we not have a direct link between prop values and properties we should
        /// find property value meta information by comparing key properties like name and value type.
        /// </summary>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public virtual bool IsSuitableForValue(PropertyValue propValue)
        {
            return string.Equals(Name, propValue.PropertyName, StringComparison.InvariantCultureIgnoreCase) && ValueType == propValue.ValueType;
        }


        public bool IsNew => IsTransient();

        #region IInheritable Members
        public virtual bool IsInherited { get; set; }

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
                foreach (var propValue in Values ?? Array.Empty<PropertyValue>())
                {
                    propValue.PropertyId = parentProperty.Id;
                    propValue.ValueType = parentProperty.ValueType;
                }
            }

            if (parent is Catalog catalog)
            {
                var displayNamesComparer = AnonymousComparer.Create((PropertyDisplayName x) => $"{x.LanguageCode}");
                var displayNamesForCatalogLanguages = catalog.Languages.Select(x => new PropertyDisplayName { LanguageCode = x.LanguageCode }).ToList();
                //Leave display names only with catalog languages
                DisplayNames = DisplayNames.Intersect(displayNamesForCatalogLanguages, displayNamesComparer).ToList();
                //Add missed
                DisplayNames.AddRange(displayNamesForCatalogLanguages.Except(DisplayNames, displayNamesComparer));
                IsManageable = true;
            }
        }
        #endregion

        #region ICloneable members
        public virtual object Clone()
        {
            return base.MemberwiseClone();
        }
        #endregion
    }
}
