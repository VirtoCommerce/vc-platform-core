using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class CatalogProduct : AuditableEntity, ILinkSupport, ISeoSupport, IHasOutlines, IHaveDimension, IHasAssociations, IHasProperties, IHasImages, IHasAssets, IInheritable, IHasTaxType
    {
        /// <summary>
        /// SKU code
        /// </summary>
        public string Code { get; set; }
        public string ManufacturerPartNumber { get; set; }
        /// <summary>
        /// Global Trade Item Number (GTIN). These identifiers include UPC (in North America), EAN (in Europe), JAN (in Japan), and ISBN (for books).
        /// </summary>
        public string Gtin { get; set; }
        public string Name { get; set; }

        public string CatalogId { get; set; }
        public Catalog Catalog { get; set; }

        public string CategoryId { get; set; }
        public Category Category { get; set; }

        public string MainProductId { get; set; }
        public CatalogProduct MainProduct { get; set; }
        public bool? IsBuyable { get; set; }
        public bool? IsActive { get; set; }
        public bool? TrackInventory { get; set; }
        public DateTime? IndexingDate { get; set; }
        public int? MaxQuantity { get; set; }
        public int? MinQuantity { get; set; }

        /// <summary>
        /// Can be Physical, Digital or Subscription.
        /// </summary>
        public string ProductType { get; set; }
        //Type of product package (set of package types with their specific dimensions)
        public string PackageType { get; set; }

        #region IHaveDimension Members
        public string WeightUnit { get; set; }
        public decimal? Weight { get; set; }

        public string MeasureUnit { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        #endregion

        public bool? EnableReview { get; set; }

        /// <summary>
        /// re-downloads limit
        /// </summary>
		public int? MaxNumberOfDownload { get; set; }
        public DateTime? DownloadExpiration { get; set; }
        /// <summary>
        /// DownloadType: {Standard Product, Software, Music}
        /// </summary>
		public string DownloadType { get; set; }
        public bool? HasUserAgreement { get; set; }

        public string ShippingType { get; set; }
        public string TaxType { get; set; }

        public string Vendor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Product order position in catalog
        /// </summary>
        public int Priority { get; set; }

        #region IHasProperties members
        public IList<Property> Properties { get; set; }
        #endregion

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

        #region IHasImages members
        public IList<Image> Images { get; set; }
        #endregion

        public IList<Asset> Assets { get; set; }

        #region IHasAssets members

        public IEnumerable<AssetBase> AllAssets
        {
            get
            {
                var result = Enumerable.Empty<AssetBase>();
                if (Images != null)
                {
                    result = result.Concat(Images);
                }
                if(Assets != null)
                {
                    result = result.Concat(Assets);
                }
                return result;            
            }
        }
        #endregion

        #region ILinkSupport members
        public IList<CategoryLink> Links { get; set; }
        #endregion

        public IList<Variation> Variations { get; set; }
        /// <summary>
        /// Each derivative type should override this property tp use other object type in seo records 
        /// </summary>
        public virtual string SeoObjectType { get; } = typeof(CatalogProduct).Name;
        public IList<SeoInfo> SeoInfos { get; set; }
        public IList<EditorialReview> Reviews { get; set; }

        #region IHasAssociations members
        public IList<ProductAssociation> Associations { get; set; }
        #endregion

        public IList<ProductAssociation> ReferencedAssociations { get; set; }

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
                //TODO: prevent saving the inherited simple values
                //TaxType  inheritance
                if (TaxType == null)
                {
                    TaxType = hasTaxType.TaxType;
                }
            }

            if (parent is CatalogProduct parentProduct)
            {
                var isVariation = GetType().IsAssignableFrom(typeof(Variation));
                //Inherit images from parent product (if its not set)
                if (Images.IsNullOrEmpty() && !parentProduct.Images.IsNullOrEmpty())
                {
                    Images = new List<Image>();
                    foreach(var parentImage in parentProduct.Images)
                    {
                        var image = AbstractTypeFactory<Image>.TryCreateInstance();
                        image.TryInheritFrom(parentImage);
                        Images.Add(image);
                    }
                }

                //Inherit assets from parent product (if its not set)
                if (Assets.IsNullOrEmpty() && !parentProduct.Assets.IsNullOrEmpty())
                {
                    Assets = new List<Asset>();
                    foreach (var parentAsset in parentProduct.Assets)
                    {
                        var asset = AbstractTypeFactory<Asset>.TryCreateInstance();
                        asset.TryInheritFrom(parentAsset);
                        Assets.Add(asset);
                    }
                }

                //inherit editorial reviews from main product and do not inherit if variation loaded within product
                if (!isVariation && Reviews.IsNullOrEmpty() && parentProduct.Reviews != null)
                {
                    Reviews = new List<EditorialReview>();
                    foreach (var parentReview in parentProduct.Reviews)
                    {
                        var review = AbstractTypeFactory<EditorialReview>.TryCreateInstance();
                        review.TryInheritFrom(parentReview);
                        Reviews.Add(review);
                    }
                }
                //inherit not overridden property values from main product
                foreach(var parentProductProperty in parentProduct.Properties)
                {                   
                    var existProperty = Properties.FirstOrDefault(x => x.IsSame(parentProductProperty, PropertyType.Product, PropertyType.Variation));
                    if(existProperty == null)
                    {
                        var property = AbstractTypeFactory<Property>.TryCreateInstance();
                        property.TryInheritFrom(parentProductProperty);
                        Properties.Add(parentProductProperty);
                    }
                }
                //TODO: prevent saving the inherited simple values
                Width = Width ?? parentProduct.MainProduct.Width;
                Height = Height ?? parentProduct.MainProduct.Height;
                Length = Length ?? parentProduct.MainProduct.Length;
                MeasureUnit = MeasureUnit ?? parentProduct.MainProduct.MeasureUnit;
                Weight = Weight ?? parentProduct.MainProduct.Weight;
                WeightUnit = WeightUnit ?? parentProduct.MainProduct.WeightUnit;
                PackageType = PackageType ?? parentProduct.MainProduct.PackageType;

                if (!Variations.IsNullOrEmpty())
                {
                    foreach(var variation in Variations)
                    {
                        variation.TryInheritFrom(this);
                    }
                }
            }
        }
        #endregion

        public virtual void ReduceDetails(string responseGroup)
        {
            //Reduce details according to response group
            var productResponseGroup = EnumUtility.SafeParse(responseGroup, ItemResponseGroup.ItemLarge);

            if (!productResponseGroup.HasFlag(ItemResponseGroup.ItemAssets))
            {
                Assets = null;
            }
            if (!productResponseGroup.HasFlag(ItemResponseGroup.ItemAssociations))
            {
                Associations = null;
            }
            if (!productResponseGroup.HasFlag(ItemResponseGroup.ReferencedAssociations))
            {
                ReferencedAssociations = null;
            }
            if (!productResponseGroup.HasFlag(ItemResponseGroup.ItemEditorialReviews))
            {
                Reviews = null;
            }         
            if (!productResponseGroup.HasFlag(ItemResponseGroup.ItemProperties))
            {
                Properties = null;
            }
            if (!productResponseGroup.HasFlag(ItemResponseGroup.Links))
            {
                Links = null;
            }
            if (!productResponseGroup.HasFlag(ItemResponseGroup.Outlines))
            {
                Outlines = null;
            }
            if (!productResponseGroup.HasFlag(ItemResponseGroup.Seo))
            {
                SeoInfos = null;
            }
            if (!productResponseGroup.HasFlag(ItemResponseGroup.Variations))
            {
                Variations = null;
            }
        }
    }
}
