using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using moduleModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class ProductConverter
    {
        public static webModel.Product ToWebModel(this moduleModel.CatalogProduct product, IBlobUrlResolver blobUrlResolver)
        {
            var retVal = AbstractTypeFactory<webModel.Product>.TryCreateInstance().FromModel(product);
            retVal.Id = product.Id;
            retVal.CatalogId = product.CatalogId;
            retVal.CategoryId = product.CategoryId;
            retVal.Code = product.Code;
            retVal.CreatedBy = product.CreatedBy;
            retVal.CreatedDate = product.CreatedDate;
            retVal.DownloadExpiration = product.DownloadExpiration;
            retVal.DownloadType = product.DownloadType;
            retVal.EnableReview = product.EnableReview;
            retVal.Gtin = product.Gtin;
            retVal.HasUserAgreement = product.HasUserAgreement;
            retVal.Height = product.Height;
            retVal.IndexingDate = product.IndexingDate;
            retVal.IsActive = product.IsActive;
            retVal.IsBuyable = product.IsBuyable;
            retVal.Length = product.Length;
            retVal.ManufacturerPartNumber = product.ManufacturerPartNumber;
            retVal.MaxNumberOfDownload = product.MaxNumberOfDownload;
            retVal.MaxQuantity = product.MaxQuantity;
            retVal.MeasureUnit = product.MeasureUnit;
            retVal.MinQuantity = product.MinQuantity;
            retVal.ModifiedBy = product.ModifiedBy;
            retVal.ModifiedDate = product.ModifiedDate;
            retVal.Name = product.Name;
            retVal.PackageType = product.PackageType;
            retVal.Priority = product.Priority;
            retVal.ProductType = product.ProductType;
            retVal.TaxType = product.TaxType;
            retVal.TrackInventory = product.TrackInventory;
            retVal.Vendor = product.Vendor;
            retVal.Weight = product.Weight;
            retVal.WeightUnit = product.WeightUnit;
            retVal.Width = product.Width;
            retVal.StartDate = product.StartDate;
            retVal.EndDate = product.EndDate;

            retVal.SeoInfos = product.SeoInfos;

            if (!product.Outlines.IsNullOrEmpty())
            {
                //Minimize outline size
                retVal.Outlines = product.Outlines.Select(x => x.ToWebModel()).ToList();
            }

            if (product.Images != null)
            {
                retVal.Images = product.Images.Select(x => x.ToWebModel(blobUrlResolver)).ToList();
            }

            if (product.Assets != null)
            {
                retVal.Assets = product.Assets.Select(x => x.ToWebModel(blobUrlResolver)).ToList();
            }

            if (product.Variations != null)
            {
                retVal.Variations = product.Variations.Select(x => x.ToWebModel(blobUrlResolver)).ToList();
                //For nested variations leave only variation properties to decrease resulting JSON
                foreach (var variation in retVal.Variations)
                {
                    if (variation.Properties != null)
                    {
                        variation.Properties = variation.Properties.Where(x => x.Type == moduleModel.PropertyType.Variation).ToList();
                    }
                }
            }

            if (product.Links != null)
            {
                retVal.Links = product.Links.Select(x => x.ToWebModel()).ToList();
            }

            if (product.Reviews != null)
            {
                retVal.Reviews = product.Reviews.Select(x => x.ToWebModel()).ToList();
            }

            if (product.Associations != null)
            {
                retVal.Associations = product.Associations.Select(x => x.ToWebModel(blobUrlResolver)).ToList();
            }

            if (product.ReferencedAssociations != null)
            {
                retVal.ReferencedAssociations = product.ReferencedAssociations.Select(a => a.ToWebModel(blobUrlResolver)).ToList();
            }
            //Init outline and path
            if (product.Category != null)
            {
                var parents = new List<moduleModel.Category>();
                if (product.Category.Parents != null)
                {
                    parents.AddRange(product.Category.Parents);
                }
                parents.Add(product.Category);

                retVal.Outline = string.Join("/", parents.Select(x => x.Id));
                retVal.Path = string.Join("/", parents.Select(x => x.Name));
            }

            retVal.TitularItemId = product.MainProductId;

            retVal.Properties = new List<webModel.Property>();
            //Need add property for each meta info
            if (product.Properties != null)
            {
                foreach (var property in product.Properties)
                {
                    var webModelProperty = property.ToWebModel();
                    webModelProperty.Values = new List<webModel.PropertyValue>();
                    webModelProperty.IsManageable = true;
                    webModelProperty.IsReadOnly = property.Type != moduleModel.PropertyType.Product && property.Type != moduleModel.PropertyType.Variation;
                    retVal.Properties.Add(webModelProperty);
                }
            }

            //Populate property values
            if (product.PropertyValues != null)
            {
                var sort = false;
                foreach (var propValue in product.PropertyValues.Select(x => x.ToWebModel()))
                {
                    var property = retVal.Properties.FirstOrDefault(x => x.Id == propValue.PropertyId);
                    if (property == null)
                    {
                        property = retVal.Properties.FirstOrDefault(x => x.Name.EqualsInvariant(propValue.PropertyName));
                    }
                    if (property == null)
                    {
                        //Need add dummy property for each value without property
                        property = new webModel.Property(propValue, product.CatalogId, moduleModel.PropertyType.Product);
                        retVal.Properties.Add(property);
                        sort = true;
                    }
                    property.Values.Add(propValue);
                }
                if (sort)
                {
                    retVal.Properties = retVal.Properties.OrderBy(x => x.Name).ToList();
                }
            }

            return retVal;
        }

        public static moduleModel.CatalogProduct ToModuleModel(this webModel.Product product, IBlobUrlResolver blobUrlResolver)
        {
            var retVal = product.ToModel(AbstractTypeFactory<moduleModel.CatalogProduct>.TryCreateInstance());
            retVal.Id = product.Id;
            retVal.CatalogId = product.CatalogId;
            retVal.CategoryId = product.CategoryId;
            retVal.Code = product.Code;
            retVal.CreatedBy = product.CreatedBy;
            retVal.CreatedDate = product.CreatedDate;
            retVal.DownloadExpiration = product.DownloadExpiration;
            retVal.DownloadType = product.DownloadType;
            retVal.EnableReview = product.EnableReview;
            retVal.Gtin = product.Gtin;
            retVal.HasUserAgreement = product.HasUserAgreement;
            retVal.Height = product.Height;
            retVal.IndexingDate = product.IndexingDate;
            retVal.IsActive = product.IsActive;
            retVal.IsBuyable = product.IsBuyable;
            retVal.Length = product.Length;
            retVal.ManufacturerPartNumber = product.ManufacturerPartNumber;
            retVal.MaxNumberOfDownload = product.MaxNumberOfDownload;
            retVal.MaxQuantity = product.MaxQuantity;
            retVal.MeasureUnit = product.MeasureUnit;
            retVal.MinQuantity = product.MinQuantity;
            retVal.ModifiedBy = product.ModifiedBy;
            retVal.ModifiedDate = product.ModifiedDate;
            retVal.Name = product.Name;
            retVal.PackageType = product.PackageType;
            retVal.Priority = product.Priority;
            retVal.ProductType = product.ProductType;
            retVal.TaxType = product.TaxType;
            retVal.TrackInventory = product.TrackInventory;
            retVal.Vendor = product.Vendor;
            retVal.Weight = product.Weight;
            retVal.WeightUnit = product.WeightUnit;
            retVal.Width = product.Width;
            retVal.StartDate = product.StartDate;
            retVal.EndDate = product.EndDate;
            retVal.SeoInfos = product.SeoInfos;

            if (product.Images != null)
            {
                retVal.Images = product.Images.Select(x => x.ToCoreModel()).ToList();
            }

            if (product.Assets != null)
            {
                retVal.Assets = product.Assets.Select(x => x.ToCoreModel()).ToList();
            }

            if (product.Properties != null)
            {
                retVal.PropertyValues = new List<moduleModel.PropertyValue>();
                foreach (var property in product.Properties)
                {
                    if (property.Values != null)
                    {
                        foreach (var propValue in property.Values)
                        {
                            //Need populate required fields
                            propValue.PropertyName = property.Name;
                            propValue.ValueType = property.ValueType;
                            retVal.PropertyValues.Add(propValue.ToCoreModel());
                        }
                    }
                }
            }

            if (product.Variations != null)
            {
                retVal.Variations = product.Variations.Select(x => x.ToModuleModel(blobUrlResolver)).ToList();
            }

            if (product.Links != null)
            {
                retVal.Links = product.Links.Select(x => x.ToCoreModel()).ToList();
            }

            if (product.Reviews != null)
            {
                retVal.Reviews = product.Reviews.Select(x => x.ToCoreModel()).ToList();
            }

            if (product.Associations != null)
            {
                retVal.Associations = product.Associations.Select(x => x.ToCoreModel()).ToList();
                var index = 0;
                foreach (var association in retVal.Associations)
                {
                    association.Priority = index++;
                }
            }
            retVal.MainProductId = product.TitularItemId;
            return retVal;
        }
    }
}
