using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
    public class PropertyDisplayName : ValueObject, IHasLanguage, ICloneable
	{
		public string Name { get; set; }
		public string LanguageCode { get; set; }

        #region ICloneable members
        public virtual object Clone()
        {
            return MemberwiseClone();
        }
        #endregion

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return LanguageCode;
        }
    }
}
