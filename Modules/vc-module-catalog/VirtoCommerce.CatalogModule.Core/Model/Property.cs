using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class Property : AuditableEntity, IInheritable, ICloneable, IHasOuterId, IHasCatalogId, IExportable
    {
        /// <summary>
        /// Gets or sets a value indicating whether user can change property value.
        /// </summary>     
        public bool IsReadOnly { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether user can change property metadata or remove this property. 
        /// </summary>
        public bool IsManageable => !IsTransient();
        /// <summary>
        /// Gets or sets a value indicating whether this instance is new. A new property should be created on server site instead of trying to update it.
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Gets or sets the catalog id that this product belongs to.
        /// </summary>
        public string CatalogId { get; set; }
        [JsonIgnore]
        public Catalog Catalog { get; set; }
        /// <summary>
        /// Gets or sets the category id that this product belongs to.
        /// </summary>
        /// <value>
        /// The category identifier.
        /// </value>
        public string CategoryId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public bool Dictionary { get; set; }
        public bool Multivalue { get; set; }
        public bool Multilanguage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Property"/> is hidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hidden; otherwise, <c>false</c>.
        /// </value>
        public bool Hidden { get; set; }

        public PropertyValueType ValueType { get; set; }
        public PropertyType Type { get; set; }
        public string OuterId { get; set; }


        public IList<PropertyValue> Values { get; set; } = new List<PropertyValue>();
        public IList<PropertyAttribute> Attributes { get; set; }
        public IList<PropertyDisplayName> DisplayNames { get; set; }
        public IList<PropertyValidationRule> ValidationRules { get; set; }
        public PropertyValidationRule ValidationRule => ValidationRules?.FirstOrDefault();

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

        #region IInheritable Members
        public virtual bool IsInherited { get; set; }

        public virtual void TryInheritFrom(IEntity parent)
        {
            if (parent is Property parentProperty)
            {
                IsInherited = true;
                Id = parentProperty.Id ?? Id;
                Name = parentProperty.Name ?? Name;
                CreatedBy = parentProperty.CreatedBy ?? CreatedBy;
                ModifiedBy = parentProperty.ModifiedBy ?? ModifiedBy;
                CreatedDate = parentProperty.CreatedDate;
                ModifiedDate = parentProperty.ModifiedDate ?? ModifiedDate;
                Required = parentProperty.Required;
                Dictionary = parentProperty.Dictionary;
                Multivalue = parentProperty.Multivalue;
                Multilanguage = parentProperty.Multilanguage;
                ValueType = parentProperty.ValueType;
                Type = parentProperty.Type;
                Attributes = parentProperty.Attributes;
                DisplayNames = parentProperty.DisplayNames;
                ValidationRules = parentProperty.ValidationRules;
                CatalogId = parentProperty.CatalogId;
                CategoryId = parentProperty.CategoryId;

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
            }
        }
        #endregion

        #region ICloneable members
        public virtual object Clone()
        {
            var result = MemberwiseClone() as Property;

            result.Values = Values?.Select(x => x.Clone()).OfType<PropertyValue>().ToList();
            result.Attributes = Attributes?.Select(x => x.Clone()).OfType<PropertyAttribute>().ToList();
            result.DisplayNames = Values?.Select(x => x.Clone()).OfType<PropertyDisplayName>().ToList();
            result.ValidationRules = Values?.Select(x => x.Clone()).OfType<PropertyValidationRule>().ToList();

            return result;
        }
        #endregion

        #region Conditional JSON serialization for properties declared in base type
        public override bool ShouldSerializeAuditableProperties => false;
        #endregion
    }
}
