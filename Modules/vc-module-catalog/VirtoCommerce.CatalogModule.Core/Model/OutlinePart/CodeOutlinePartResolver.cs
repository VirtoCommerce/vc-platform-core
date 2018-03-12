using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.OutlinePart
{
    /// <summary>
    /// Uses codes for outline items.
    /// </summary>
    public class CodeOutlinePartResolver : IOutlinePartResolver
    {
        public string ResolveOutlinePart(IEntity entity)
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
