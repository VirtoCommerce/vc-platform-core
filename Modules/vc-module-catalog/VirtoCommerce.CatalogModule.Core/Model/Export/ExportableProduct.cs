using System.Linq;
using VirtoCommerce.CoreModule.Core.Outlines;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.Export
{
    public class ExportableProduct : CatalogProduct, IExportable, IExportViewable, ITabularConvertible
    {
        public string ImageUrl { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }

        public virtual ExportableProduct FromModel(CatalogProduct source)
        {
            Type = nameof(CatalogProduct);
            Id = source.Id;
            Code = source.Code;
            ManufacturerPartNumber = source.ManufacturerPartNumber;
            Gtin = source.Gtin;
            Name = source.Name;
            CatalogId = source.CatalogId;
            CategoryId = source.CategoryId;
            MainProductId = source.MainProductId;
            IsBuyable = source.IsBuyable;
            IsActive = source.IsActive;
            TrackInventory = source.TrackInventory;
            IndexingDate = source.IndexingDate;
            MaxQuantity = source.MaxQuantity;
            MinQuantity = source.MinQuantity;
            ProductType = source.ProductType;
            PackageType = source.PackageType;
            WeightUnit = source.WeightUnit;
            Weight = source.Weight;
            MeasureUnit = source.MeasureUnit;
            Height = source.Height;
            Length = source.Length;
            Width = source.Width;
            EnableReview = source.EnableReview;
            MaxNumberOfDownload = source.MaxNumberOfDownload;
            DownloadExpiration = source.DownloadExpiration;
            DownloadType = source.DownloadType;
            HasUserAgreement = source.HasUserAgreement;
            ShippingType = source.ShippingType;
            TaxType = source.TaxType;
            Vendor = source.Vendor;
            StartDate = source.StartDate;
            EndDate = source.EndDate;
            Priority = source.Priority;
            OuterId = source.OuterId;

            Images = source.Images?.Select(x => x.Clone() as Image).ToList();
            Assets = source.Assets?.Select(x => x.Clone() as Asset).ToList();
            Links = source.Links?.Select(x => x.Clone() as CategoryLink).ToList();
            Variations = source.Variations?.Select(x => x.Clone() as Variation).ToList();
            SeoInfos = source.SeoInfos?.Select(x => x.Clone() as SeoInfo).ToList();
            Reviews = source.Reviews?.Select(x => x.Clone() as EditorialReview).ToList();
            Associations = source.Associations?.Select(x => x.Clone() as ProductAssociation).ToList();
            ReferencedAssociations = source.ReferencedAssociations?.Select(x => x.Clone() as ProductAssociation).ToList();
            Outlines = source.Outlines?.Select(x => x.Clone() as Outline).ToList();
            Properties = source.Properties?.Select(x => x.Clone() as Property).ToList();

            return this;
        }

        #region ITabularConvertible implementation

        public virtual IExportable ToTabular()
        {
            var result = AbstractTypeFactory<TabularProduct>.TryCreateInstance();

            result.Id = Id;
            result.Code = Code;
            result.ManufacturerPartNumber = ManufacturerPartNumber;
            result.Gtin = Gtin;
            result.Name = Name;
            result.CatalogId = CatalogId;
            result.CategoryId = CategoryId;
            result.MainProductId = MainProductId;
            result.IsBuyable = IsBuyable;
            result.IsActive = IsActive;
            result.TrackInventory = TrackInventory;
            result.IndexingDate = IndexingDate;
            result.MaxQuantity = MaxQuantity;
            result.MinQuantity = MinQuantity;
            result.ProductType = ProductType;
            result.PackageType = PackageType;
            result.WeightUnit = WeightUnit;
            result.Weight = Weight;
            result.MeasureUnit = MeasureUnit;
            result.Height = Height;
            result.Length = Length;
            result.Width = Width;
            result.EnableReview = EnableReview;
            result.MaxNumberOfDownload = MaxNumberOfDownload;
            result.DownloadExpiration = DownloadExpiration;
            result.DownloadType = DownloadType;
            result.HasUserAgreement = HasUserAgreement;
            result.ShippingType = ShippingType;
            result.TaxType = TaxType;
            result.Vendor = Vendor;
            result.StartDate = StartDate;
            result.EndDate = EndDate;
            result.Priority = Priority;
            result.OuterId = OuterId;

            return result;
        }
        #endregion ITabularConvertible implementation
    }
}

