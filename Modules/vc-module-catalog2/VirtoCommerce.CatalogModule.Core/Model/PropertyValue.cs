using System;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
    public class PropertyValue : AuditableEntity, IHasLanguage, IInheritable, ICloneable
    {
        public string PropertyId { get; set; }
        public string PropertyName { get; set; }

        public string Alias { get; set; }
        public string ValueId { get; set; }

        public object Value { get; set; }

        public PropertyValueType ValueType { get; set; }
        public string LanguageCode { get; set; }

        public override string ToString()
        {
            return (PropertyName ?? "unknown") + "-" + (Value ?? "undefined");
        }

        #region IInheritable Members
        public bool IsInherited { get; private set; }
        public virtual void TryInheritFrom(IEntity parent)
        {
            var parentPropertyValue = parent as PropertyValue;
            if (parent is PropertyValue parentPropValue)
            {
                Id = null;
                IsInherited = true;
                PropertyId = parentPropValue.PropertyId;
                PropertyName = parentPropValue.PropertyName;
                Alias = parentPropValue.Alias;
                ValueId = parentPropValue.ValueId;
                ValueType = parentPropValue.ValueType;
                LanguageCode = parentPropValue.LanguageCode;
            }
        }
        #endregion

        #region ICloneable members
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}
