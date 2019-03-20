using System;
using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class CategoryItemRelationEntity : Entity
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

            link.CategoryId = CategoryId;
            link.CatalogId = CatalogId;
            link.Priority = Priority;

            return link;
        }

        public virtual CategoryItemRelationEntity FromModel(CategoryLink link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            CategoryId = link.CategoryId;
            CatalogId = link.CatalogId;
            Priority = link.Priority;

            return this;
        }

        public virtual void Patch(CategoryItemRelationEntity target)
        {
            //Nothing todo. Because we not support change  link
        }

    }

    public class CategoryItemRelationComparer : IEqualityComparer<CategoryItemRelationEntity>
    {
        #region IEqualityComparer<CategoryItemRelation> Members

        public bool Equals(CategoryItemRelationEntity x, CategoryItemRelationEntity y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode(CategoryItemRelationEntity obj)
        {
            var hash = 17;
            hash = hash * 23 + obj.CatalogId.GetHashCode();
            hash = hash * 23 + obj.Priority.GetHashCode();
            if (obj.CategoryId != null)
            {
                hash = hash * 23 + obj.CategoryId.GetHashCode();
            }
            return hash;
        }

        #endregion
    }
}
