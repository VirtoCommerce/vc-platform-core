using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;
using System.Linq;
using System;

namespace VirtoCommerce.Domain.Catalog.Model
{
    public class Catalog : Entity, IHasProperties, ICloneable
    {
        public string Name { get; set; }
        public bool IsVirtual { get; set; }
        public CatalogLanguage DefaultLanguage
        {
            get
            {
                CatalogLanguage retVal = null;
                if (Languages != null)
                {
                    retVal = Languages.FirstOrDefault(x => x.IsDefault);
                }
                return retVal;
            }
        }
        public ICollection<CatalogLanguage> Languages { get; set; }

        #region IHasProperties
        public ICollection<Property> Properties { get; set; }
        public ICollection<PropertyValue> PropertyValues { get; set; }
        #endregion

        #region ICloneable members
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}
