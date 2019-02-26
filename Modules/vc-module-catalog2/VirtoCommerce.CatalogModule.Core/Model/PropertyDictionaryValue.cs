using System;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
    /// <summary>
    /// Represent dictionary property values 
    /// </summary>
	public class PropertyDictionaryValue : Entity, IHasLanguage, ICloneable
	{
        /// <summary>
        /// Property identifier
        /// </summary>
		public string PropertyId { get; set; }
		public Property Property { get; set; }
        /// <summary>
        /// Alias for value used for group same dict values in different languages
        /// </summary>
		public string Alias { get; set; }
        /// <summary>
        /// Language
        /// </summary>
		public string LanguageCode { get; set; }
		public string Value { get; set; }

        #region ICloneable members
        public virtual object Clone()
        {
            return MemberwiseClone();
        } 
        #endregion
    }
}
