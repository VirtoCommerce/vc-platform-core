using System;
using System.Collections.Generic;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class CategoryRelationEntity : Entity
    {
        #region Navigation Properties
        public string SourceCategoryId { get; set; }
        public virtual CategoryEntity SourceCategory { get; set; }

        public string TargetCatalogId { get; set; }
        public virtual CatalogEntity TargetCatalog { get; set; }

        public string TargetCategoryId { get; set; }
        public virtual CategoryEntity TargetCategory { get; set; }
        #endregion

        public virtual CategoryLink ToModel(CategoryLink link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            link.CategoryId = TargetCategoryId;
            link.CatalogId = TargetCatalogId;

            return link;
        }

        public virtual CategoryRelationEntity FromModel(CategoryLink link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            TargetCategoryId = link.CategoryId;
            TargetCatalogId = link.CatalogId;

            return this;
        }

        public virtual void Patch(CategoryRelationEntity target)
        {
            //Nothing todo. Because we not support change  link
        }

    }

    public class LinkedCategoryComparer : IEqualityComparer<CategoryRelationEntity>
    {
        #region IEqualityComparer<LinkedCategory> Members

        public bool Equals(CategoryRelationEntity x, CategoryRelationEntity y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode(CategoryRelationEntity obj)
        {
            var hash = 17 * 23;
            if (obj.TargetCategoryId != null)
            {
                hash = hash * 23 + obj.TargetCategoryId.GetHashCode();
            }
            else if (obj.TargetCatalogId != null)
            {
                hash = hash * 23 + obj.TargetCatalogId.GetHashCode();
            }
            return hash;
        }

        #endregion
    }
}
