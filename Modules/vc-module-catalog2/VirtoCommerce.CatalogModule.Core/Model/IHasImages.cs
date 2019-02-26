using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
    public interface IHasImages
    {
        string ImgSrc { get;  }
        IList<Image> Images { get; }
    }
}
