using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
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
            ImageUrl = category.Images.FirstOrDefault()?.Url;
            Code = category.Code;
            Name = category.Name;
            IsActive = category.IsActive;

            if (!category.Outlines.IsNullOrEmpty())
            {
                Outline = category.Outlines.Select(x => x.ToString()).ToArray();
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
