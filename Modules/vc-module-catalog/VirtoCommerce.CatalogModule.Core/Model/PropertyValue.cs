using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;
namespace VirtoCommerce.Domain.Catalog.Model
{
    public class PropertyValue : AuditableEntity, IHasLanguage, IInheritable, ICloneable
    {
        public string PropertyId { get; set; }
        public string PropertyName { get; set; }
        public Property Property { get; set; }
        public string Alias { get; set; }
        public string ValueId { get; set; }

        private object _value;
        public object Value
        {
            get
            {
                var retVal = _value;
                /// Return actual dictionary property value from property meta-information instead stored
                if (Property != null && Property.Dictionary && Property.DictionaryValues != null && ValueId != null)
                {
                    var dictValue = Property.DictionaryValues.FirstOrDefault(x => x.Id == ValueId);
                    if (dictValue != null)
                    {
                        retVal = dictValue.Value;
                    }

                }
                return retVal;
            }
            set
            {
                _value = value;
            }
        }

        public PropertyValueType ValueType { get; set; }
        public string LanguageCode { get; set; }

        public override string ToString()
        {
            return (PropertyName ?? "unknown") + ":" + (Value ?? "undefined");
        }


        /// <summary>
        /// Returns for current value all dictionary values in all defined languages 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PropertyValue> TryGetAllLocalizedDictValues()
        {
            var retVal = new List<PropertyValue>();

            if (Property != null && Property.Dictionary && Property.Multilanguage && Property.DictionaryValues != null)
            {
                foreach (var dictValue in Property.DictionaryValues.Where(x => x.Alias == Alias))
                {
                    var langDictPropValue = this.Clone() as PropertyValue;
                    langDictPropValue.Id = null;
                    langDictPropValue.LanguageCode = dictValue.LanguageCode;
                    langDictPropValue.Value = dictValue.Value;
                    langDictPropValue.ValueId = dictValue.Id;
                    retVal.Add(langDictPropValue);
                }
            }
            return retVal;
        }

        #region IInheritable Members
        public bool IsInherited { get; set; }
        #endregion

        #region ICloneable members
        public object Clone()
        {
            return base.MemberwiseClone();
        }
        #endregion
    }
}