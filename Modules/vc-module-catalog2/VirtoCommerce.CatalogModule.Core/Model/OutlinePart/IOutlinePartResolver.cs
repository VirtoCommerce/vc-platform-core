using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.OutlinePart
{
    /// <summary>
    /// Resolves an outline part for an entity.
    /// Abstraction allows us to switch behavior e.g. use codes instead of ids.
    /// </summary>
    public interface IOutlinePartResolver
    {
        string ResolveOutlinePart(IEntity entity);
    }
}
