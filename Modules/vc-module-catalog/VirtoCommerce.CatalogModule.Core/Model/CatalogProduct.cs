using System;
using System.Collections.Generic;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Domain.Catalog.Model
{
    public class CatalogProduct : AuditableEntity, ILinkSupport, ISeoSupport, IHasOutlines, IHaveDimension, IHasAssociations, IHasProperties, IHasImages, IHasAssets
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
        public ICollection<Property> Properties { get; set; }
        public ICollection<PropertyValue> PropertyValues { get; set; }

        #endregion
        #region IHasImages members
        public ICollection<Image> Images { get; set; }
        #endregion

        #region IHasAssets members
        public ICollection<Asset> Assets { get; set; }
        #endregion

        #region ILinkSupport members
        public ICollection<CategoryLink> Links { get; set; }
        #endregion

        public ICollection<CatalogProduct> Variations { get; set; }
        /// <summary>
        /// Each derivative type should override this property tp use other object type in seo records 
        /// </summary>
        public virtual string SeoObjectType { get; } = typeof(CatalogProduct).Name;
        public ICollection<SeoInfo> SeoInfos { get; set; }
        public ICollection<EditorialReview> Reviews { get; set; }

        #region IHasAssociations members
        public ICollection<ProductAssociation> Associations { get; set; }
        #endregion

        public ICollection<ProductAssociation> ReferencedAssociations { get; set; }

        #region IHasOutlines members
        public ICollection<Outline> Outlines { get; set; }
        #endregion
    }
}
