using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface IHasImages
    {
        IList<Image> Images { get; }
    }
}
