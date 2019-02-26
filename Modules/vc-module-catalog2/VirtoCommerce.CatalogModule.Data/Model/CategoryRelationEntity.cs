using System;
using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core2.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data2.Model
{
    public class CategoryRelationEntity : ValueObject
    {
        public string Id { get; set; }

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

            link.EntryId = SourceCategoryId;
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

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TargetCatalogId;
            yield return TargetCategoryId;
        }
    }
  
}
