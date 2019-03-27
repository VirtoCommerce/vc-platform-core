using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface IHasImages
    {
        string ImgSrc { get; }
        IList<Image> Images { get; set; }
    }
}
