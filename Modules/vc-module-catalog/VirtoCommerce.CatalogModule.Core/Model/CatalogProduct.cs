using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CatalogModule.Core.Serialization;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Outlines;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class CatalogProduct : AuditableEntity, IHasLinks, ISeoSupport, IHasOutlines, IHasDimension, IHasAssociations, IHasProperties, IHasImages, IHasAssets, IInheritable, IHasTaxType, ICloneable
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
        [JsonIgnore]
        public Catalog Catalog { get; set; }

        public string CategoryId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }

        public string TitularItemId => MainProductId;
        public string MainProductId { get; set; }
        [JsonIgnore]
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
        [JsonIgnoreSerialization]
        [Obsolete("it's for importing data from v.2, need to use values in Properties")]
        public ICollection<PropertyValue> PropertyValues { get; set; }

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

        #region IHasAssets members
        public IList<Asset> Assets { get; set; }
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

        public IList<InventoryInfo> Inventories { get; set; }

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
                    var existProperty = Properties.FirstOrDefault(x => x.IsSame(parentProperty, PropertyType.Product, PropertyType.Variation));
                    if (existProperty != null)
                    {
                        existProperty.TryInheritFrom(parentProperty);
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
                    foreach (var parentImage in parentProduct.Images)
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
                foreach (var parentProductProperty in parentProduct.Properties ?? Array.Empty<Property>())
                {
                    var existProperty = Properties.FirstOrDefault(x => x.IsSame(parentProductProperty, PropertyType.Product, PropertyType.Variation));
                    if (existProperty == null)
                    {
                        var property = AbstractTypeFactory<Property>.TryCreateInstance();
                        property.TryInheritFrom(parentProductProperty);
                        Properties.Add(parentProductProperty);
                    }
                }
                //TODO: prevent saving the inherited simple values
                Width = parentProduct.Width ?? Width;
                Height = parentProduct.Height ?? Height;
                Length = parentProduct.Length ?? Length;
                MeasureUnit = parentProduct.MeasureUnit ?? MeasureUnit;
                Weight = parentProduct.Weight ?? Weight;
                WeightUnit = parentProduct.WeightUnit ?? WeightUnit;
                PackageType = parentProduct.PackageType ?? PackageType;

                if (!Variations.IsNullOrEmpty())
                {
                    foreach (var variation in Variations)
                    {
                        variation.TryInheritFrom(this);
                    }
                }
            }
        }
        #endregion

        public virtual CatalogProduct GetCopy()
        {
            var result = Clone() as CatalogProduct;

            // Clear ID for all related entities except properties
            var allEntities = this.GetFlatObjectsListWithInterface<ISeoSupport>();
            foreach (var entity in allEntities)
            {
                var property = entity as Property;
                if (property is null)
                {
                    entity.SeoInfos.Clear();
                    entity.Id = null;
                }
            }
            return result;
        }

        public virtual void Move(string catalogId, string categoryId)
        {
            CatalogId = catalogId;
            CategoryId = categoryId;
            foreach (var variation in Variations)
            {
                variation.CatalogId = catalogId;
                variation.CategoryId = categoryId;
            }
        }

        public virtual void ReduceDetails(string responseGroup)
        {
            //Reduce details according to response group
            var productResponseGroup = EnumUtility.SafeParseFlags(responseGroup, ItemResponseGroup.ItemLarge);

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
            if (Variations != null)
            {
                //For nested variations leave only variation properties to decrease resulting JSON
                foreach (var variation in Variations)
                {
                    if (variation.Properties != null)
                    {
                        variation.Properties = variation.Properties.Where(x => x.Type == PropertyType.Variation).ToList();
                    }
                    variation.Outlines = null;
                    variation.Reviews = null;
                }
            }
        }


        #region ICloneable members
        public virtual object Clone()
        {
            var result = MemberwiseClone() as CatalogProduct;

            result.SeoInfos = SeoInfos?.Select(x => x.Clone()).OfType<SeoInfo>().ToList();
            result.Images = Images?.Select(x => x.Clone()).OfType<Image>().ToList();
            result.Assets = Assets?.Select(x => x.Clone()).OfType<Asset>().ToList();
            result.Properties = Properties?.Select(x => x.Clone()).OfType<Property>().ToList();
            result.Associations = Associations?.Select(x => x.Clone()).OfType<ProductAssociation>().ToList();
            result.ReferencedAssociations = ReferencedAssociations?.Select(x => x.Clone()).OfType<ProductAssociation>().ToList();
            result.Reviews = Reviews?.Select(x => x.Clone()).OfType<EditorialReview>().ToList();
            result.Links = Links?.Select(x => x.Clone()).OfType<CategoryLink>().ToList();
            result.Variations = Variations?.Select(x => x.Clone()).OfType<Variation>().ToList();

            return result;
        }
        #endregion
    }
}
