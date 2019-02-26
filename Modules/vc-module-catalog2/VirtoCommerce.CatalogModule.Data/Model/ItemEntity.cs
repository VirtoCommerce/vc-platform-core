using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class ItemEntity : AuditableEntity
    {
        public ItemEntity()
        {
            CategoryLinks = new NullCollection<CategoryItemRelationEntity>();
            Images = new NullCollection<ImageEntity>();
            Assets = new NullCollection<AssetEntity>();
            EditorialReviews = new NullCollection<EditorialReviewEntity>();
            ItemPropertyValues = new NullCollection<PropertyValueEntity>();
            Childrens = new NullCollection<ItemEntity>();
            Associations = new NullCollection<AssociationEntity>();
            ReferencedAssociations = new NullCollection<AssociationEntity>();
        }

        [StringLength(1024)]
        [Required]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public bool IsBuyable { get; set; }

        public int AvailabilityRule { get; set; }

        public decimal MinQuantity { get; set; }

        public decimal MaxQuantity { get; set; }

        public bool TrackInventory { get; set; }


        [StringLength(128)]
        public string PackageType { get; set; }

        [StringLength(64)]
        [Required]
        public string Code { get; set; }

        [StringLength(128)]
        public string ManufacturerPartNumber { get; set; }
        [StringLength(64)]
        public string Gtin { get; set; }

        [StringLength(64)]
        public string ProductType { get; set; }

        [StringLength(32)]
        public string WeightUnit { get; set; }
        public decimal? Weight { get; set; }
        [StringLength(32)]
        public string MeasureUnit { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }

        public bool? EnableReview { get; set; }

        public int? MaxNumberOfDownload { get; set; }
        public DateTime? DownloadExpiration { get; set; }
        [StringLength(64)]
        public string DownloadType { get; set; }
        public bool? HasUserAgreement { get; set; }
        [StringLength(64)]
        public string ShippingType { get; set; }
        [StringLength(64)]
        public string TaxType { get; set; }
        [StringLength(128)]
        public string Vendor { get; set; }

        public int Priority { get; set; }

        #region Navigation Properties

        public virtual ObservableCollection<CategoryItemRelationEntity> CategoryLinks { get; set; }

        public virtual ObservableCollection<AssetEntity> Assets { get; set; }

        public virtual ObservableCollection<ImageEntity> Images { get; set; }

        public virtual ObservableCollection<AssociationEntity> Associations { get; set; }

        public virtual ObservableCollection<AssociationEntity> ReferencedAssociations { get; set; }

        public virtual ObservableCollection<EditorialReviewEntity> EditorialReviews { get; set; }

        public virtual ObservableCollection<PropertyValueEntity> ItemPropertyValues { get; set; }

        public string CatalogId { get; set; }
        public virtual CatalogEntity Catalog { get; set; }

        public string CategoryId { get; set; }
        public virtual CategoryEntity Category { get; set; }

        public string ParentId { get; set; }
        public virtual ItemEntity Parent { get; set; }

        public virtual ObservableCollection<ItemEntity> Childrens { get; set; }
        #endregion


        public virtual CatalogProduct ToModel(CatalogProduct product, bool convertChildrens = true, bool convertAssociations = true)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));


            product.Id = Id;
            product.CreatedDate = CreatedDate;
            product.CreatedBy = CreatedBy;
            product.ModifiedDate = ModifiedDate;
            product.ModifiedBy = ModifiedBy;

            product.CatalogId = CatalogId;
            product.CategoryId = CategoryId;
            product.Code = Code;
            product.DownloadExpiration = DownloadExpiration;
            product.DownloadType = DownloadType;
            product.EnableReview = EnableReview;
            product.EndDate = EndDate;
            product.Gtin = Gtin;
            product.HasUserAgreement = HasUserAgreement;
            product.Height = Height;
            product.IsActive = IsActive;
            product.IsBuyable = IsBuyable;
            product.Length = Length;
            product.MainProductId = ParentId;
            product.ManufacturerPartNumber = ManufacturerPartNumber;
            product.MaxNumberOfDownload = MaxNumberOfDownload;
            product.MaxQuantity = (int)MaxQuantity;
            product.MeasureUnit = MeasureUnit;
            product.MinQuantity = (int)MinQuantity;
            product.Name = Name;
            product.PackageType = PackageType;
            product.Priority = Priority;
            product.ProductType = ProductType;
            product.ShippingType = ShippingType;
            product.StartDate = StartDate;
            product.TaxType = TaxType;
            product.TrackInventory = TrackInventory;
            product.Vendor = Vendor;
            product.Weight = Weight;
            product.WeightUnit = WeightUnit;
            product.Width = Width;

            //Links
            product.Links = CategoryLinks.Select(x => x.ToModel(AbstractTypeFactory<CategoryLink>.TryCreateInstance())).ToList();
            //Images
            product.Images = Images.OrderBy(x => x.SortOrder).Select(x => x.ToModel(AbstractTypeFactory<Image>.TryCreateInstance())).ToList();
            //Assets
            product.Assets = Assets.OrderBy(x => x.CreatedDate).Select(x => x.ToModel(AbstractTypeFactory<Asset>.TryCreateInstance())).ToList();
            // EditorialReviews
            product.Reviews = EditorialReviews.Select(x => x.ToModel(AbstractTypeFactory<EditorialReview>.TryCreateInstance())).ToList();

            if (convertAssociations)
            {
                // Associations
                product.Associations = Associations.Select(x => x.ToModel(AbstractTypeFactory<ProductAssociation>.TryCreateInstance())).OrderBy(x => x.Priority).ToList();
                product.ReferencedAssociations = ReferencedAssociations.Select(x => x.ToReferencedAssociationModel(AbstractTypeFactory<ProductAssociation>.TryCreateInstance())).OrderBy(x => x.Priority).ToList();
            }

            product.Properties = new List<Property>();
            foreach (var propValues in ItemPropertyValues.GroupBy(x => x.Name))
            {   
                //Need add property (without meta information) for each values group with the same property name
                var property = AbstractTypeFactory<Property>.TryCreateInstance();
                property.Name = propValues.Key;
                property.Type = PropertyType.Product;
                property.Values = propValues.OrderBy(x => x.Id).Select(x => x.ToModel(AbstractTypeFactory<PropertyValue>.TryCreateInstance())).ToList();
                product.Properties.Add(property);
            }
          
            if (Parent != null)
            {
                product.MainProduct = Parent.ToModel(AbstractTypeFactory<CatalogProduct>.TryCreateInstance(), false, convertAssociations);
            }

            if (convertChildrens)
            {
                // Variations
                product.Variations = new List<Variation>();
                foreach (var variation in Childrens)
                {
                    var productVariation = variation.ToModel(AbstractTypeFactory<Variation>.TryCreateInstance()) as Variation;
                    productVariation.MainProduct = product;
                    productVariation.MainProductId = product.Id;
                    product.Variations.Add(productVariation);
                }
            }
            return product;
        }

        public virtual ItemEntity FromModel(CatalogProduct product, PrimaryKeyResolvingMap pkMap)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            pkMap.AddPair(product, this);

            Id = product.Id;
            CreatedDate = product.CreatedDate;
            CreatedBy = product.CreatedBy;
            ModifiedDate = product.ModifiedDate;
            ModifiedBy = product.ModifiedBy;

            CatalogId = product.CatalogId;
            CategoryId = product.CategoryId;
            Code = product.Code;
            DownloadExpiration = product.DownloadExpiration;
            DownloadType = product.DownloadType;
            EnableReview = product.EnableReview;
            EndDate = product.EndDate;
            Gtin = product.Gtin;
            HasUserAgreement = product.HasUserAgreement;
            Height = product.Height;
            IsActive = product.IsActive ?? true;
            IsBuyable = product.IsBuyable ?? true;
            Length = product.Length;
            ParentId = product.MainProductId;
            ManufacturerPartNumber = product.ManufacturerPartNumber;
            MaxNumberOfDownload = product.MaxNumberOfDownload;
            MaxQuantity = product.MaxQuantity ?? 0;
            MeasureUnit = product.MeasureUnit;
            MinQuantity = product.MinQuantity ?? 0;
            Name = product.Name;
            PackageType = product.PackageType;
            Priority = product.Priority;
            ProductType = product.ProductType;
            ShippingType = product.ShippingType;
            TaxType = product.TaxType;
            TrackInventory = product.TrackInventory ?? true;
            Vendor = product.Vendor;
            Weight = product.Weight;
            WeightUnit = product.WeightUnit;
            Width = product.Width;

            StartDate = product.StartDate == default(DateTime) ? DateTime.UtcNow : product.StartDate;

            //Constant fields
            //Only for main product
            AvailabilityRule = (int)Core.Model.AvailabilityRule.Always;

            CatalogId = product.CatalogId;
            CategoryId = string.IsNullOrEmpty(product.CategoryId) ? null : product.CategoryId;

            #region ItemPropertyValues
            if (product.Properties != null)
            {
                ItemPropertyValues = new ObservableCollection<PropertyValueEntity>();
                foreach (var propertyValue in product.Properties.SelectMany(x => x.Values))
                {
                    if (!propertyValue.IsInherited && propertyValue.Value != null && !string.IsNullOrEmpty(propertyValue.Value.ToString()))
                    {
                        ItemPropertyValues.Add(AbstractTypeFactory<PropertyValueEntity>.TryCreateInstance().FromModel(propertyValue, pkMap));
                    }
                }
            }
            #endregion

            #region Assets
            if (product.Assets != null)
            {
                Assets = new ObservableCollection<AssetEntity>(product.Assets.Where(x => !x.IsInherited).Select(x => AbstractTypeFactory<AssetEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }
            #endregion

            #region Images
            if (product.Images != null)
            {
                Images = new ObservableCollection<ImageEntity>(product.Images.Where(x => !x.IsInherited).Select(x => AbstractTypeFactory<ImageEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }
            #endregion

            #region Links
            if (product.Links != null)
            {
                CategoryLinks = new ObservableCollection<CategoryItemRelationEntity>(product.Links.Select(x => AbstractTypeFactory<CategoryItemRelationEntity>.TryCreateInstance().FromModel(x)));
            }
            #endregion

            #region EditorialReview
            if (product.Reviews != null)
            {
                EditorialReviews = new ObservableCollection<EditorialReviewEntity>(product.Reviews.Where(x => !x.IsInherited).Select(x => AbstractTypeFactory<EditorialReviewEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }
            #endregion

            #region Associations
            if (product.Associations != null)
            {
                Associations = new ObservableCollection<AssociationEntity>(product.Associations.Select(x => AbstractTypeFactory<AssociationEntity>.TryCreateInstance().FromModel(x)));
            }
            #endregion

            if (product.Variations != null)
            {
                Childrens = new ObservableCollection<ItemEntity>(product.Variations.Select(x => AbstractTypeFactory<ItemEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }

            return this;
        }

        public virtual void Patch(ItemEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.IsBuyable = IsBuyable;
            target.IsActive = IsActive;
            target.TrackInventory = TrackInventory;
            target.MinQuantity = MinQuantity;
            target.MaxQuantity = MaxQuantity;
            target.EnableReview = EnableReview;

            target.CatalogId = CatalogId;
            target.CategoryId = CategoryId;
            target.Name = Name;
            target.Code = Code;
            target.ManufacturerPartNumber = ManufacturerPartNumber;
            target.Gtin = Gtin;
            target.ProductType = ProductType;
            target.MaxNumberOfDownload = MaxNumberOfDownload;
            target.DownloadType = DownloadType;
            target.HasUserAgreement = HasUserAgreement;
            target.DownloadExpiration = DownloadExpiration;
            target.Vendor = Vendor;
            target.TaxType = TaxType;
            target.WeightUnit = WeightUnit;
            target.Weight = Weight;
            target.MeasureUnit = MeasureUnit;
            target.PackageType = PackageType;
            target.Height = Height;
            target.Length = Length;
            target.Width = Width;
            target.ShippingType = ShippingType;
            target.Priority = Priority;
            target.ParentId = ParentId;
            target.StartDate = StartDate;
            target.EndDate = EndDate;

            #region Assets
            if (!Assets.IsNullCollection())
            {
                Assets.Patch(target.Assets, (sourceAsset, targetAsset) => sourceAsset.Patch(targetAsset));
            }
            #endregion

            #region Images
            if (!Images.IsNullCollection())
            {
                Images.Patch(target.Images, (sourceImage, targetImage) => sourceImage.Patch(targetImage));
            }
            #endregion

            #region ItemPropertyValues
            if (!ItemPropertyValues.IsNullCollection())
            {
                ItemPropertyValues.Patch(target.ItemPropertyValues, (sourcePropValue, targetPropValue) => sourcePropValue.Patch(targetPropValue));
            }
            #endregion

            #region Links
            if (!CategoryLinks.IsNullCollection())
            {
                CategoryLinks.Patch(target.CategoryLinks, (sourcePropValue, targetPropValue) => sourcePropValue.Patch(targetPropValue));
            }
            #endregion

            #region EditorialReviews
            if (!EditorialReviews.IsNullCollection())
            {
                EditorialReviews.Patch(target.EditorialReviews, (sourcePropValue, targetPropValue) => sourcePropValue.Patch(targetPropValue));
            }
            #endregion

            #region Association
            if (!Associations.IsNullCollection())
            {
                var associationComparer = AnonymousComparer.Create((AssociationEntity x) => x.AssociationType + ":" + x.AssociatedItemId + ":" + x.AssociatedCategoryId);
                Associations.Patch(target.Associations, associationComparer,
                                             (sourceAssociation, targetAssociation) => sourceAssociation.Patch(targetAssociation));
            }
            #endregion
        }
    }
}
