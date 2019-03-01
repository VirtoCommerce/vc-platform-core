using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web.Model
{
    /// <summary>
    /// Individual dictionary value record for dictionary supporting property.
    /// </summary>
    public class PropertyDictionaryValue : Entity
    {
        /// <summary>
        /// Gets or sets the property id that this dictionary value belongs to.
        /// </summary>
        /// <value>
        /// The property identifier.
        /// </value>
        public string PropertyId { get; set; }
        /// <summary>
        /// Gets or sets the value of this dictionary value in default language.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public string Alias { get; set; }
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public string LanguageCode { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
        /// <summary>
        /// Dictionary item identifier
        /// </summary>
        public string ValueId { get; set; }
    }
}
