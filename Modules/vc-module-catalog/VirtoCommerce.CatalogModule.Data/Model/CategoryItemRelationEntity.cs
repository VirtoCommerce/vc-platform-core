using System;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class CategoryItemRelationEntity : Entity, ICloneable
    {
        public int Priority { get; set; }

        #region Navigation Properties

        public string ItemId { get; set; }
        public virtual ItemEntity CatalogItem { get; set; }

        public string CategoryId { get; set; }
        public virtual CategoryEntity Category { get; set; }

        public string CatalogId { get; set; }
        public virtual CatalogEntity Catalog { get; set; }

        #endregion

        public virtual CategoryLink ToModel(CategoryLink link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            link.ListEntryId = ItemId;
            link.CategoryId = CategoryId;
            link.CatalogId = CatalogId;
            link.Priority = Priority;

            return link;
        }

        public virtual CategoryItemRelationEntity FromModel(CategoryLink link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            ItemId = link.ListEntryId;
            CategoryId = link.CategoryId;
            CatalogId = link.CatalogId;
            Priority = link.Priority;

            return this;
        }

        public virtual void Patch(CategoryItemRelationEntity target)
        {
            target.Priority = Priority;
        }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as CategoryItemRelationEntity;

            if (CatalogItem != null)
            {
                result.CatalogItem = CatalogItem.Clone() as ItemEntity;
            }

            if (Category != null)
            {
                result.Category = Category.Clone() as CategoryEntity;
            }

            if (Catalog != null)
            {
                result.Catalog = Catalog.Clone() as CatalogEntity;
            }

            return result;
        }

        #endregion
    }
}
