using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
	public class CategoryLink : ValueObject, ICloneable
    {
        /// <summary>
        /// Entry identifier which this link belongs to
        /// </summary>
        public string EntryId { get; set; }
        /// <summary>
        /// Product order position in virtual catalog
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// The catalog identifier which link referenced to
        /// </summary>
		public string CatalogId { get; set; }
		public Catalog Catalog { get; set; }
        /// <summary>
        /// The category identifier which link referenced to
        /// </summary>
        public string CategoryId { get; set; }
		public Category Category { get; set; }

        #region ICloneable members
        public virtual object Clone()
        {
            return MemberwiseClone();
        } 
        #endregion

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return EntryId;
            yield return CatalogId;
            yield return CategoryId;
        }
    }
}
