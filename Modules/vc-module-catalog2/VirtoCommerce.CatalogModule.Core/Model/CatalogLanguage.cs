
using System.Globalization;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class CatalogLanguage : Entity, IHasLanguage
    {
        /// <summary>
        /// Gets or sets the catalog identifier.
        /// </summary>
        /// <value>
        /// The catalog identifier.
        /// </value>
        public string CatalogId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this catalog language is default.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets the human-readable language name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName
        {
            get
            {
                var retVal = LanguageCode;

                try
                {
                    var culture = CultureInfo.CreateSpecificCulture(LanguageCode);
                    return culture.DisplayName;
                }
                catch
                {
                    return retVal;
                }
            }
        }
        
    }
}
