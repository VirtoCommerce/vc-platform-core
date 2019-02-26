
using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface IHasAssets
    {
        IEnumerable<AssetBase> AllAssets { get; }
    }
}
