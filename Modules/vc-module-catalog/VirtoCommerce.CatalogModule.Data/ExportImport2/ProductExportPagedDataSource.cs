using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class ProductExportPagedDataSource : ExportPagedDataSource<ProductExportDataQuery, ProductSearchCriteria>
    {
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IItemService _itemService;
        private readonly IProductSearchService _productSearchService;

        public ProductExportPagedDataSource(IBlobStorageProvider blobStorageProvider, IItemService itemService, IProductSearchService productSearchService, ProductExportDataQuery dataQuery) : base(dataQuery)
        {
            _blobStorageProvider = blobStorageProvider;
            _itemService = itemService;
            _productSearchService = productSearchService;
        }

        protected override ExportableSearchResult FetchData(ProductSearchCriteria searchCriteria)
        {
            CatalogProduct[] result;
            int totalCount;

            var responseGroup = BuildResponseGroup();

            if (searchCriteria.ObjectIds.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                result = _itemService.GetByIdsAsync(searchCriteria.ObjectIds.ToArray(), responseGroup.ToString()).GetAwaiter().GetResult();
                totalCount = result.Length;
            }
            else
            {
                searchCriteria.ResponseGroup = responseGroup.ToString();
                var productSearchResult = _productSearchService.SearchProductsAsync(searchCriteria).GetAwaiter().GetResult();
                result = productSearchResult.Results.ToArray();
                totalCount = productSearchResult.TotalCount;
            }

            if (DataQuery.IncludedProperties.Any(x => x.FullName.Contains("Images.BinaryData")))
            {
                LoadImages(result);
            }


            return new ExportableSearchResult()
            {
                Results = ToExportable(result).ToList(),
                TotalCount = totalCount
            };

        }

        private ItemResponseGroup BuildResponseGroup()
        {
            var result = ItemResponseGroup.ItemInfo;

            if (DataQuery.IncludedProperties.Any(x => x.FullName.StartsWith(nameof(CatalogProduct.Assets) + ".")))
            {
                result |= ItemResponseGroup.WithImages;
            }

            if (DataQuery.IncludedProperties.Any(x => x.FullName.StartsWith(nameof(CatalogProduct.Properties) + ".")))
            {
                result |= ItemResponseGroup.WithProperties;
            }

            if (DataQuery.IncludedProperties.Any(x => x.FullName.StartsWith(nameof(CatalogProduct.Associations) + ".")))
            {
                result |= ItemResponseGroup.ItemAssociations;
            }

            if (DataQuery.IncludedProperties.Any(x => x.FullName.StartsWith(nameof(CatalogProduct.Variations) + ".")))
            {
                result |= ItemResponseGroup.WithVariations;
            }

            if (DataQuery.IncludedProperties.Any(x => x.FullName.StartsWith(nameof(CatalogProduct.SeoInfos) + ".")))
            {
                result |= ItemResponseGroup.WithSeo;
            }

            if (DataQuery.IncludedProperties.Any(x => x.FullName.StartsWith(nameof(CatalogProduct.Links) + ".")))
            {
                result |= ItemResponseGroup.WithLinks;
            }

            if (DataQuery.IncludedProperties.Any(x => x.FullName.StartsWith(nameof(CatalogProduct.ReferencedAssociations) + ".")))
            {
                result |= ItemResponseGroup.ReferencedAssociations;
            }

            if (DataQuery.IncludedProperties.Any(x => x.FullName.StartsWith(nameof(CatalogProduct.Outlines) + ".")))
            {
                result |= ItemResponseGroup.WithOutlines;
            }

            if (DataQuery.IncludedProperties.Any(x => x.FullName.StartsWith(nameof(CatalogProduct.Reviews) + ".")))
            {
                result |= ItemResponseGroup.ItemEditorialReviews;
            }

            return result;
        }

        protected virtual IEnumerable<IExportable> ToExportable(IEnumerable<ICloneable> objects)
        {
            var models = objects.Cast<CatalogProduct>();
            var viewableMap = models.ToDictionary(x => x, x => AbstractTypeFactory<ExportableProduct>.TryCreateInstance().FromModel(x));

            //FillAdditionalProperties(viewableMap);

            var modelIds = models.Select(x => x.Id).ToList();

            return viewableMap.Values.OrderBy(x => modelIds.IndexOf(x.Id));
        }


        protected virtual void FillAdditionalProperties(Dictionary<CatalogProduct, ExportableProduct> viewableMap)
        {
            //var models = viewableMap.Keys;
            //var productIds = models.Select(x => x.ProductId).Distinct().ToArray();
            //var pricelistIds = models.Select(x => x.PricelistId).Distinct().ToArray();
            //var products = _itemService.GetByIdsAsync(productIds, ItemResponseGroup.ItemInfo.ToString()).GetAwaiter().GetResult();
            //var pricelists = _pricingService.GetPricelistsByIdAsync(pricelistIds).GetAwaiter().GetResult();

            // foreach (var kvp in viewableMap)
            // {



            //    var model = kvp.Key;
            //    var viewableEntity = kvp.Value;
            //    var product = products.FirstOrDefault(x => x.Id == model.ProductId);
            //    var pricelist = pricelists.FirstOrDefault(x => x.Id == model.PricelistId);

            //    viewableEntity.Code = product?.Code;
            //    viewableEntity.ImageUrl = product?.ImgSrc;
            //    viewableEntity.Name = product?.Name;
            //    viewableEntity.ProductName = product?.Name;
            //    viewableEntity.Parent = pricelist?.Name;
            //    viewableEntity.PricelistName = pricelist?.Name;
            // }
        }


        private void LoadImages(IHasImages[] haveImagesObjects)
        {
            var allImages = haveImagesObjects.SelectMany(x => x.GetFlatObjectsListWithInterface<IHasImages>())
                .SelectMany(x => x.Images).ToArray();
            foreach (var image in allImages)
            {
                using (var stream = _blobStorageProvider.OpenRead(image.Url))
                {
                    image.BinaryData = stream.ReadFully();
                }
            }
        }

        protected override ProductSearchCriteria BuildSearchCriteria(ProductExportDataQuery exportDataQuery)
        {
            var result = base.BuildSearchCriteria(exportDataQuery);

            result.CatalogId = exportDataQuery.CatalogId;
            result.CategoryId = exportDataQuery.CategoryId;
            result.SearchInVariations = exportDataQuery.SearchInVariations;
            result.ProductTypes = exportDataQuery.ProductTypes;
            result.Skus = exportDataQuery.Skus;

            return result;
        }
    }
}
