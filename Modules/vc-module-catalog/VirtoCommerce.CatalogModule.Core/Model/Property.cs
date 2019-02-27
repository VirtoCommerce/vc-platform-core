using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
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
        public ICollection<PropertyAttribute> Attributes { get; set; }
        public ICollection<PropertyDisplayName> DisplayNames { get; set; }
        public ICollection<PropertyValidationRule> ValidationRules { get; set; }

        /// <summary>
        /// Because we not have a direct link between prop values and properties we should
        /// find property value meta information by comparing key properties like name and value type.
        /// </summary>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public bool IsSuitableForValue(PropertyValue propValue)
        {
            return String.Equals(Name, propValue.PropertyName, StringComparison.InvariantCultureIgnoreCase) && ValueType == propValue.ValueType;
        }

        #region IInheritable Members
        public bool IsInherited { get; set; }

        public virtual void TryInheritFrom(IEntity parent)
        {
            if (parent is Property parentBase)
            {
                Id = null;
                IsInherited = true;
                Name = parentBase.Name;
                CatalogId = parentBase.CatalogId;
                Category = parentBase.Category;
                Required = parentBase.Required;
                Dictionary = parentBase.Dictionary;
                Multivalue = parentBase.Multivalue;
                Multilanguage = parentBase.Multilanguage;
                ValueType = parentBase.ValueType;
                Type = parentBase.Type;
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
