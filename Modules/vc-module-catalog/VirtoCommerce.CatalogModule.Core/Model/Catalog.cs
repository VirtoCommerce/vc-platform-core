using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;
using System.Linq;
using System;

namespace VirtoCommerce.CatalogModule.Core.Model
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
        public IList<CatalogLanguage> Languages { get; set; }

        #region IHasProperties
        public IList<Property> Properties { get; set; }
        #endregion

        #region ICloneable members
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}
