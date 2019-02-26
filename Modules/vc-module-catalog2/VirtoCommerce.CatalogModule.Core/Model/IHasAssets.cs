
using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
    public interface IHasAssets
    {
        IEnumerable<AssetBase> AllAssets { get; }
    }
}
