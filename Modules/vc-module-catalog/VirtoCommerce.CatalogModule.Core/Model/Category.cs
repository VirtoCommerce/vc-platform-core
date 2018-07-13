using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class Category : AuditableEntity, IHasLinks, ISeoSupport, IHasOutlines, IHasImages, IHasProperties, ICloneable, IHasAssets, IHasTaxType, IInheritable
    {
        public Category()
        {
            IsActive = true;
        }
        public string CatalogId { get; set; }
        public Catalog Catalog { get; set; }

        public string ParentId { get; set; }
        public Category Parent { get; set; }
        public string Code { get; set; }
        public string TaxType { get; set; }
        public string Name { get; set; }
        public bool IsVirtual { get; set; }
        public int Level { get; set; }
        public IList<Category> Parents { get; set; }

        //Type of product package (set of package types with their specific dimensions) can be inherited by nested products and categories
        public string PackageType { get; set; }

        public int Priority { get; set; }

        public bool? IsActive { get; set; }

        public IList<Category> Children { get; set; }

        #region IHasProperties members
        public IList<Property> Properties { get; set; }
        #endregion

        #region ILinkSupport members
        public IList<CategoryLink> Links { get; set; }
        #endregion

        public string SeoObjectType { get { return GetType().Name; } }
        public IList<SeoInfo> SeoInfos { get; set; }

        #region IHasImages members
        /// <summary>
        /// Gets the default image for the product.
        /// </summary>
        /// <value>
        /// The image source URL.
        /// </value>
        public string ImgSrc
        {
            get
            {
                string result = null;
                if (Images != null && Images.Any())
                {
                    result = Images.OrderBy(x => x.SortOrder).FirstOrDefault()?.Url;
                }
                return result;
            }
        }

        public IList<Image> Images { get; set; }
        #endregion

        #region IHasOutlines members
        public IList<Outline> Outlines { get; set; }
        #endregion

        #region ICloneable
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion

        #region IHasAssets members
        public IEnumerable<AssetBase> AllAssets
        {
            get
            {
                return Images?.OfType<AssetBase>();
            }
        }
        #endregion

        #region IInheritable members
        /// <summary>
        /// System flag used to mark that object was inherited from other
        /// </summary>
        public bool IsInherited { get; private set; }

        public virtual void TryInheritFrom(IEntity parent)
        {
            if (parent is IHasProperties hasProperties)
            {
                //Properties inheritance
                foreach (var parentProperty in hasProperties.Properties)
                {
                    var existProperty = Properties.FirstOrDefault(x => x.IsSame(parentProperty, PropertyType.Product, PropertyType.Variation));
                    if (existProperty != null)
                    {
                        existProperty.TryInheritFrom(parentProperty);
                        existProperty.ActualizeValues();
                    }
                    else
                    {
                        Properties.Add(parentProperty);
                    }
                }
            }

            if (parent is IHasTaxType hasTaxType)
            {
                //Try to inherit taxType from parent category
                if (TaxType == null)
                {
                    TaxType = hasTaxType.TaxType;
                }
            }
        }
        #endregion

        public virtual void Move(string catalogId, string categoryId)
        {
            CatalogId = catalogId;
            ParentId = categoryId;
        }

        public virtual void ReduceDetails(string responseGroup)
        {
            //Reduce details according to response group
            var categoryResponseGroup = EnumUtility.SafeParse(responseGroup, CategoryResponseGroup.Full);

            if (!categoryResponseGroup.HasFlag(CategoryResponseGroup.WithImages))
            {
                Images = null;
            }
            if (!categoryResponseGroup.HasFlag(CategoryResponseGroup.WithLinks))
            {
                Links = null;
            }
            if (!categoryResponseGroup.HasFlag(CategoryResponseGroup.WithParents))
            {
                Parents = null;
            }
            if (!categoryResponseGroup.HasFlag(CategoryResponseGroup.WithProperties))
            {
                Properties = null;
            }
            if (!categoryResponseGroup.HasFlag(CategoryResponseGroup.WithOutlines))
            {
                Outlines = null;
            }
            if (!categoryResponseGroup.HasFlag(CategoryResponseGroup.WithSeo))
            {
                SeoInfos = null;
            }
        }
    }
}
