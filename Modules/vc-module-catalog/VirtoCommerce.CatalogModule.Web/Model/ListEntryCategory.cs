using System.Linq;

namespace VirtoCommerce.CatalogModule.Web.Model
{
    /// <summary>
    /// Category ListEntry record.
    /// </summary>
    public class ListEntryCategory : ListEntry
    {
        public const string TypeName = "category";

        public ListEntryCategory(Category category)
            : base(TypeName, category)
        {
            ImageUrl = category.ImgSrc;
            Code = category.Code;
            Name = category.Name;
            IsActive = category.IsActive;

            if (!string.IsNullOrEmpty(category.Outline))
            {
                Outline = category.Outline.Split('/').Select(x => x).ToArray();
            }

            if (!string.IsNullOrEmpty(category.Path))
            {
                Path = category.Path.Split('/').Select(x => x).ToArray();
            }

            if (category.Links != null)
            {
                Links = category.Links.Select(x => new ListEntryLink(x)).ToArray();
            }
        }
    }
}
