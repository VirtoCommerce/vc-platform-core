using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Tools.Models;

namespace VirtoCommerce.Tools
{
    public static class OutlineExtensions
    {
        /// <summary>
        /// Returns best matching outline path for the given catalog: CategoryId/CategoryId2.
        /// </summary>
        /// <param name="outlines"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public static string GetOutlinePath(this IEnumerable<Outline> outlines, string catalogId)
        {
            var result = string.Empty;

            // Find any outline for the given catalog
            var outline = outlines?.GetOutlineForCatalog(catalogId);
            if (outline != null)
            {
                var pathSegments = new List<string>();

                pathSegments.AddRange(outline.Items
                    .Where(i => i.SeoObjectType != "Catalog")
                    .Select(i => i.Id));

                if (pathSegments.All(s => s != null))
                {
                    result = string.Join("/", pathSegments);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns first outline for the given catalog (if any)
        /// </summary>
        /// <param name="outlines"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public static Outline GetOutlineForCatalog(this IEnumerable<Outline> outlines, string catalogId)
        {
            // Find any outline for the given catalog
            var result = outlines?.FirstOrDefault(o => o.Items.Any(i => i.SeoObjectType == "Catalog" && i.Id == catalogId));
            return result;
        }
    }
}
