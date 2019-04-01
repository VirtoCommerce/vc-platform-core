using System;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class PropertyValue : AuditableEntity, IHasLanguage, IInheritable, ICloneable
    {
        public string PropertyId { get; set; }
        public string PropertyName { get; set; }
        [JsonIgnore]
        public Property Property { get; set; }
        public string Alias { get; set; }
        public string ValueId { get; set; }
        public object Value { get; set; }

        public PropertyValueType ValueType { get; set; }
        public string LanguageCode { get; set; }

        public override string ToString()
        {
            return (PropertyName ?? "unknown") + ":" + (Value ?? "undefined");
        }


        #region IInheritable Members
        public bool IsInherited { get; set; }

        public virtual void TryInheritFrom(IEntity parent)
        {
            if (parent is PropertyValue parentBase)
            {
                Id = null;
                IsInherited = true;
                LanguageCode = parentBase.LanguageCode;
                PropertyId = parentBase.PropertyId;
                PropertyName = parentBase.PropertyName;
                Property = parentBase.Property;
                Alias = parentBase.Alias;
                ValueId = parentBase.ValueId;
                Value = parentBase.Value;
                ValueType = parentBase.ValueType;
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
