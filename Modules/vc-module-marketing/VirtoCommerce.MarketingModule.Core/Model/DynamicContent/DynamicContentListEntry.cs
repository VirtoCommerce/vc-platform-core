using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model
{
    public abstract class DynamicContentListEntry : AuditableEntity, ICloneable
    {
        /// <summary>
        /// Gets or sets the type. E.g. "folder", "content-item", "content-place"
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public virtual string ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        public string Description { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            return MemberwiseClone() as DynamicContentListEntry;
        }

        #endregion
    }
}
