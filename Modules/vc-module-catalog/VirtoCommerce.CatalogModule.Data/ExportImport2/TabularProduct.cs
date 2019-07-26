using System;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class TabularProduct
    {
        public string Code { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Gtin { get; set; }
        public string OuterId { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public string CatalogId { get; set; }
        public string CategoryId { get; set; }
        public string MainProductId { get; set; }
        public bool? IsBuyable { get; set; }
        public bool? IsActive { get; set; }
        public bool? TrackInventory { get; set; }
        public DateTime? IndexingDate { get; set; }
        public int? MaxQuantity { get; set; }
        public int? MinQuantity { get; set; }
        public string ProductType { get; set; }
        public string PackageType { get; set; }
        public string WeightUnit { get; set; }
        public decimal? Weight { get; set; }

        public string MeasureUnit { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }


        public bool? EnableReview { get; set; }

        public int? MaxNumberOfDownload { get; set; }
        public DateTime? DownloadExpiration { get; set; }
        public string DownloadType { get; set; }
        public bool? HasUserAgreement { get; set; }

        public string ShippingType { get; set; }
        public string TaxType { get; set; }

        public string Vendor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int Priority { get; set; }

    }
}
