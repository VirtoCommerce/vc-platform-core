using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class Catalog : AuditableEntity, IHasProperties, ICloneable
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

        #region IHasProperties members
        public IList<Property> Properties { get; set; }

        #endregion

        #region ICloneable members
        public virtual object Clone()
        {
            var result = MemberwiseClone() as Catalog;
            result.Languages = Languages?.Select(x => x.Clone()).OfType<CatalogLanguage>().ToList();
            result.Properties = Properties?.Select(x => x.Clone()).OfType<Property>().ToList();
            return result;
        }

        #endregion
    }
}
