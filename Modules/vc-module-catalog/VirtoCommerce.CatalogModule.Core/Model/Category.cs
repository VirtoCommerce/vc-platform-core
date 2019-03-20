using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Outlines;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class Category : AuditableEntity, ILinkSupport, ISeoSupport, IHasOutlines, IHasImages, IHasProperties
    {
        public Category()
        {
            IsActive = true;
        }
        public string CatalogId { get; set; }
        public Catalog Catalog { get; set; }

        public string ParentId { get; set; }
        public string Code { get; set; }
        public string TaxType { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsVirtual { get; set; }
        public int Level { get; set; }
        public Category[] Parents { get; set; }

        //Type of product package (set of package types with their specific dimensions) can be inherited by nested products and categories
        public string PackageType { get; set; }

        public int Priority { get; set; }

        public bool? IsActive { get; set; }

        public ICollection<Category> Children { get; set; }

        #region IHasProperties members
        public ICollection<Property> Properties { get; set; }

        #endregion

        #region ILinkSupport members
        public ICollection<CategoryLink> Links { get; set; }
        #endregion

        public string SeoObjectType { get { return GetType().Name; } }
        public IList<SeoInfo> SeoInfos { get; set; }

        #region IHasImages members
        public ICollection<Image> Images { get; set; }
        #endregion

        #region IHasOutlines members
        public ICollection<Outline> Outlines { get; set; }
        #endregion


        public virtual Category MemberwiseCloneCategory()
        {
            var retVal = AbstractTypeFactory<Category>.TryCreateInstance();

            // Entity
            retVal.Id = Id;

            // AuditableEntity
            retVal.CreatedDate = CreatedDate;
            retVal.ModifiedDate = ModifiedDate;
            retVal.CreatedBy = CreatedBy;
            retVal.ModifiedBy = ModifiedBy;

            // Category
            retVal.CatalogId = CatalogId;
            retVal.Code = Code;
            retVal.IsActive = IsActive;
            retVal.IsVirtual = IsVirtual;
            retVal.Level = Level;
            retVal.Name = Name;
            retVal.PackageType = PackageType;
            retVal.ParentId = ParentId;
            retVal.Path = Path;
            retVal.Priority = Priority;
            retVal.TaxType = TaxType;

            // TODO: clone reference objects
            retVal.Children = Children;
            retVal.Outlines = Outlines;
            retVal.SeoInfos = SeoInfos;
            retVal.Catalog = Catalog;
            retVal.Properties = Properties;
            retVal.Parents = Parents;
            retVal.Links = Links;
            retVal.Images = Images;

            return retVal;
        }
    }
}
