using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

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
        public ICollection<CatalogLanguage> Languages { get; set; }

        #region IHasProperties members
        public ICollection<Property> Properties { get; set; }

        #endregion

        #region ICloneable members
        public object Clone()
        {
            var retVal = base.MemberwiseClone() as Catalog;
            return retVal;
        }

        public virtual Catalog MemberwiseCloneCatalog()
        {
            var retVal = AbstractTypeFactory<Catalog>.TryCreateInstance();

            retVal.Id = Id;
            retVal.IsVirtual = IsVirtual;
            retVal.Name = Name;

            // TODO: clone reference objects
            retVal.Languages = Languages;
            retVal.Properties = Properties;

            return retVal;
        }
        #endregion
    }
}
