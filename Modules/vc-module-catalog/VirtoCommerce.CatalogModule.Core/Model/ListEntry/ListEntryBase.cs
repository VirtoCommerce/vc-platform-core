using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.ListEntry
{
    /// <summary>
    /// Base class for all entries used in catalog categories browsing.
    /// </summary>
	public class ListEntryBase : AuditableEntity, ISeoSupport
    {
        /// <summary>
        /// Gets or sets the type. E.g. "product", "category"
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is active.
        /// </summary>
		public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
		public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the entry code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
		public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
		public string Name { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
		public IList<CategoryLink> Links { get; set; }

        /// <summary>
        /// All entry parents ids
        /// </summary>
        public IList<string> Outline { get; set; }

        /// <summary>
        /// All entry parents names
        /// </summary>
        public IList<string> Path { get; set; }

        /// <summary>
        /// Gets or sets the catalog id.
        /// </summary>
        public string CatalogId { get; set; }

        #region ISeoSupport members
        public virtual string SeoObjectType { get; set; }

        public virtual IList<SeoInfo> SeoInfos { get; set; }
        #endregion

        public virtual ListEntryBase FromModel(AuditableEntity entity)
        {
            // Entity
            Id = entity.Id;

            // AuditableEntity
            CreatedDate = entity.CreatedDate;
            ModifiedDate = entity.ModifiedDate;
            CreatedBy = entity.CreatedBy;
            ModifiedBy = entity.ModifiedBy;

            if (entity is ISeoSupport seoSupport)
            {
                SeoObjectType = seoSupport.SeoObjectType;
                SeoInfos = seoSupport.SeoInfos;
            }

            return this;
        }
    }
}
