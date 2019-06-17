using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.ListEntry
{
    /// <summary>
    /// Category ListEntry record.
    /// </summary>
    public class CategoryListEntry : ListEntryBase
    {
        public override ListEntryBase FromModel(AuditableEntity entity)
        {
            base.FromModel(entity);

            if (entity is Category category)
            {
                Type = "category";
                ImageUrl = category.ImgSrc;
                Code = category.Code;
                Name = category.Name;
                IsActive = category.IsActive;
                Links = category.Links;
                CatalogId = category.CatalogId;

                if (!category.Outlines.IsNullOrEmpty())
                {
                    //TODO:  Use only physical catalog outline which this category belongs to
                    Outline = category.Outlines.FirstOrDefault().Items.Select(x => x.Id).ToList();
                    Path = category.Outlines.FirstOrDefault().Items.Select(x => x.Name).ToList();
                }
            }
            return this;
        }
    }
}
