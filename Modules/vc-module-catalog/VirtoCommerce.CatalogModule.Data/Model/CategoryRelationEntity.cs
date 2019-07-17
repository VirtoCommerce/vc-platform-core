using System;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class CategoryRelationEntity : Entity, ICloneable
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
            //Nothing todo. Because we not support change link
        }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as CategoryRelationEntity;

            if (SourceCategory != null)
            {
                result.SourceCategory = SourceCategory.Clone() as CategoryEntity;
            }

            if (TargetCatalog != null)
            {
                result.TargetCatalog = TargetCatalog.Clone() as CatalogEntity;
            }

            if (TargetCategory != null)
            {
                result.TargetCategory = TargetCategory.Clone() as CategoryEntity;
            }

            return result;
        }

        #endregion
    }
}
