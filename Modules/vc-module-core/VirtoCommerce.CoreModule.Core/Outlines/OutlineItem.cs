using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Seo;

namespace VirtoCommerce.CoreModule.Core.Outlines
{
    /// <summary>
    /// Represents one outline element: catalog, category or product.
    /// </summary>
    public class OutlineItem : ISeoSupport
    {
        #region ISeoSupport Members

        /// <summary>
        /// Object id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Object type
        /// </summary>
        public string SeoObjectType { get; set; }

        /// <summary>
        /// All SEO records for the object
        /// </summary>
        public IList<SeoInfo> SeoInfos { get; set; }

        #endregion
        /// <summary>
        /// The name of current item
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// True when this object is linked to the virtual parent.
        /// </summary>
        public bool HasVirtualParent { get; set; }

        public override string ToString()
        {
            return (HasVirtualParent ? "*" : "") + Id;
        }
    }
}
