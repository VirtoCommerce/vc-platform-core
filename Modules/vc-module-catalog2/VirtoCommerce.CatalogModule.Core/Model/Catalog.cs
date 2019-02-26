using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
    public class Catalog : AuditableEntity, IAggregateRoot, IHasProperties, ICloneable
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
