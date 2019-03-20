using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Services.OutlineParts
{
    /// <summary>
    /// Resolves an outline part for an entity.
    /// Abstraction allows us to switch behavior e.g. use codes instead of ids.
    /// </summary>
    public interface IOutlinePartResolver
    {
        string ResolveOutlinePart(Entity entity);
    }
}