using System;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport.Converters
{
    public class TabularProductDataConverter : ITabularDataConverter
    {
        public object ToTabular(object obj)
        {
            var source = obj as CatalogProduct ?? throw new ArgumentException(nameof(obj));
            var result = AbstractTypeFactory<TabularProduct>.TryCreateInstance();

            result.Code = source.Code;
            result.Id = source.Id;
            result.Name = source.Name;
            result.Gtin = source.Gtin;
            result.OuterId = source.OuterId;
            result.ManufacturerPartNumber = source.ManufacturerPartNumber;
            result.CatalogId = source.CatalogId;
            result.CategoryId = source.CategoryId;
            result.MainProductId = source.MainProductId;
            result.IsBuyable = source.IsBuyable;
            result.IsActive = source.IsActive;
            result.TrackInventory = source.TrackInventory;
            result.IndexingDate = source.IndexingDate;
            result.MaxQuantity = source.MaxQuantity;
            result.MinQuantity = source.MinQuantity;
            result.ProductType = source.ProductType;
            result.PackageType = source.PackageType;
            result.WeightUnit = source.WeightUnit;
            result.Weight = source.Weight;
            result.MeasureUnit = source.MeasureUnit;
            result.Height = source.Height;
            result.Length = source.Length;
            result.Width = source.Width;
            result.EnableReview = source.EnableReview;
            result.MaxNumberOfDownload = source.MaxNumberOfDownload;
            result.DownloadExpiration = source.DownloadExpiration;
            result.DownloadType = source.DownloadType;
            result.HasUserAgreement = source.HasUserAgreement;
            result.ShippingType = source.ShippingType;
            result.TaxType = source.TaxType;
            result.Vendor = source.Vendor;
            result.StartDate = source.StartDate;
            result.EndDate = source.EndDate;
            result.Priority = source.Priority;

            return result;
        }
    }
}
