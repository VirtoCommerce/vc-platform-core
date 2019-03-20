using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class PropertyDictionaryItemLocalizedValue : ValueObject, IHasLanguage
    {
        #region ILanguageSupport members
        public string LanguageCode { get; set; }
        #endregion
        public string Value { get; set; }
    }
}
