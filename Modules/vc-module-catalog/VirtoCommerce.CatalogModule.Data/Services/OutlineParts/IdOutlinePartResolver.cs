using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Services.OutlineParts
{
    /// <summary>
    /// Uses ids for oultines.
    /// </summary>
    public class IdOutlinePartResolver : IOutlinePartResolver
    {
        public string ResolveOutlinePart(Entity entity)
        {
            return entity.Id;
        }
    }
}