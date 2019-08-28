using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Outlines;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class Category : AuditableEntity, IHasLinks, ISeoSupport, IHasOutlines, IHasImages, IHasProperties, ICloneable, IHasTaxType, IHasName, IHasOuterId, IHasCatalogId, IExportable
    {
        public Category()
        {
            IsActive = true;
        }
        public string CatalogId { get; set; }
        [JsonIgnore]
        public Catalog Catalog { get; set; }

        public string ParentId { get; set; }
        [JsonIgnore]
        public Category Parent { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// Category outline in physical catalog (all parent categories ids concatenated. E.g. (1/21/344))
        /// </summary>
        public string Outline => Parent != null ? $"{Parent.Outline}/{Id}" : Id;
        /// <summary>
        /// Category path in physical catalog (all parent categories names concatenated. E.g. (parent1/parent2))
        /// </summary>
        public string Path => Parent != null ? $"{Parent.Path}/{Name}" : Name;


        public bool IsVirtual { get; set; }
        public int Level { get; set; }
        [JsonIgnore]
        public Category[] Parents { get; set; }

        //Type of product package (set of package types with their specific dimensions) can be inherited by nested products and categories
        public string PackageType { get; set; }

        public int Priority { get; set; }

        public bool? IsActive { get; set; }
        public string OuterId { get; set; }
        [JsonIgnore]
        public IList<Category> Children { get; set; }

        #region IHasProperties members
        public IList<Property> Properties { get; set; }

        #endregion

        #region ILinkSupport members
        public IList<CategoryLink> Links { get; set; }
        #endregion

        #region IHasTaxType members
        public string TaxType { get; set; }
        #endregion
        public string SeoObjectType { get { return GetType().Name; } }
        public IList<SeoInfo> SeoInfos { get; set; }

        #region IHasImages members
        /// <summary>
        /// Gets the default image
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
                foreach (var parentProperty in hasProperties.Properties ?? Array.Empty<Property>())
                {
                    if (Properties == null)
                    {
                        Properties = new List<Property>();
                    }
                    //Try to find property by type and name
                    var existProperty = Properties.FirstOrDefault(x => x.IsSame(parentProperty, PropertyType.Product, PropertyType.Variation));
                    if (existProperty == null)
                    {
                        existProperty = AbstractTypeFactory<Property>.TryCreateInstance();
                        Properties.Add(existProperty);
                    }
                    existProperty.TryInheritFrom(parentProperty);
                    existProperty.IsReadOnly = existProperty.Type != PropertyType.Category;
                }
                //Restore order after changes
                if (Properties != null)
                {
                    Properties = Properties.OrderBy(x => x.Name).ToList();
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

        #region ICloneable
        public virtual object Clone()
        {
            return MemberwiseClone();
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
            var categoryResponseGroup = EnumUtility.SafeParseFlags(responseGroup, CategoryResponseGroup.Full);

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
