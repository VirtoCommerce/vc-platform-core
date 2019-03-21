using System.Linq;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    /// <summary>
    /// Product ListEntry record.
    /// </summary>
	public class ListEntryProduct : ListEntry
    {
        public const string TypeName = "product";
        public string ProductType { get; set; }

        public ListEntryProduct(CatalogProduct product, IBlobUrlResolver blobUrlResolver)
            : base(TypeName, product)
        {
            ProductType = product.ProductType;

            if (!product.Images.IsNullOrEmpty())
            {
                ImageUrl = blobUrlResolver.GetAbsoluteUrl(product.Images.FirstOrDefault()?.Url);
            }

            Code = product.Code;
            Name = product.Name;
            IsActive = product.IsActive ?? true;

            if (!product.Outlines.IsNullOrEmpty())
            {
                Outline = product.Outlines.Select(x => x.ToString()).ToArray();
            }

            //TODO
            //if (!string.IsNullOrEmpty(product))
            //{
            //    Path = product.Path.Split('/').Select(x => x).ToArray();
            //}

            if (product.Links != null)
            {
                Links = product.Links.Select(x => new ListEntryLink(x)).ToArray();
            }
        }
    }
}
