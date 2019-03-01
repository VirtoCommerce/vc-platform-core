using System.Linq;

namespace VirtoCommerce.CatalogModule.Web.Model
{
    /// <summary>
    /// Product ListEntry record.
    /// </summary>
	public class ListEntryProduct : ListEntry
    {
        public const string TypeName = "product";
        public string ProductType { get; set; }

        public ListEntryProduct(Product product)
            : base(TypeName, product)
        {
            ProductType = product.ProductType;

            ImageUrl = product.ImgSrc;
            Code = product.Code;
            Name = product.Name;
            IsActive = product.IsActive ?? true;

            if (!string.IsNullOrEmpty(product.Outline))
            {
                Outline = product.Outline.Split('/').Select(x => x).ToArray();
            }

            if (!string.IsNullOrEmpty(product.Path))
            {
                Path = product.Path.Split('/').Select(x => x).ToArray();
            }

            if (product.Links != null)
            {
                Links = product.Links.Select(x => new ListEntryLink(x)).ToArray();
            }
        }
    }
}
