using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Services.OutlineParts
{
    /// <summary>
    /// Uses codes for outline items.
    /// </summary>
    public class CodeOutlinePartResolver : IOutlinePartResolver
    {
        public string ResolveOutlinePart(Entity entity)
        {
            // Fall-back to id.
            var result = entity.Id;
            if (entity is Category category)
            {
                result = category.Code;
            }
            if (entity is CatalogProduct product)
            {
                result = product.Code;
            }
            return result;
        }
    }
}