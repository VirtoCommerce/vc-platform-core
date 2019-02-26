using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core2.Model;

namespace VirtoCommerce.CatalogModule.Core2.Services
{
    /// <summary>
    /// Constructs outlines for objects
    /// </summary>
    public interface IOutlineService
    {
        /// <summary>
        /// Constructs single physical and/or multiple virtual outlines for given objects.
        /// Outline is the path from the catalog to one of the child objects (product or category):
        /// catalog/parent-category1/.../parent-categoryN/object
        /// </summary>
        /// <param name="objects">Objects for which outlines should be constructed.</param>
        /// <param name="catalogId">If catalogId is not null then only outlines starting with this catalog will be constructed. If catalogId is null then all possible outlines will be constructed.</param>
        void FillOutlinesForObjects(IEnumerable<IHasOutlines> objects, string catalogId);
    }
}
