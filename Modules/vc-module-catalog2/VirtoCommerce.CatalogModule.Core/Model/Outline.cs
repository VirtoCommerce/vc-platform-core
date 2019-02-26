using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
    /// <summary>
    /// Represents the path from the catalog to one of the child objects (product or category):
    /// catalog/parent-category1/.../parent-categoryN/object
    /// </summary>
    public class Outline
    {
        /// <summary>
        /// Outline parts
        /// </summary>
        public IList<OutlineItem> Items { get; set; }

        public override string ToString()
        {
            return Items == null ? null : string.Join("/", Items);
        }
    }
}
