using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class PropertyDisplayName : ValueObject, IHasLanguage
    {
        public string Name { get; set; }
        #region IHasLanguage members
        public string LanguageCode { get; set; }
        #endregion
    }
}
