using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Export;
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

            var responseGroup = !string.IsNullOrEmpty(searchCriteria.ResponseGroup) ? searchCriteria.ResponseGroup : BuildResponseGroup();

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

            if (DataQuery.LoadImageBinaries == true || DataQuery.IncludedProperties.Any(x => x.FullName.Contains("Images.BinaryData")))
            {
                LoadImages(result);
            }

            return new ExportableSearchResult()
            {
                Results = ToExportable(result).ToList(),
                TotalCount = totalCount
            };

        }

        protected virtual IEnumerable<IExportable> ToExportable(IEnumerable<ICloneable> objects)
        {
            var models = objects.Cast<CatalogProduct>();

            var exportableProducts = models.Select(x =>
            {
                var exportableProduct = AbstractTypeFactory<ExportableProduct>.TryCreateInstance().FromModel(x);
                exportableProduct.ImageUrl = x.ImgSrc;
                return exportableProduct;
            });

            return exportableProducts;
        }

        protected override ProductSearchCriteria BuildSearchCriteria(ProductExportDataQuery exportDataQuery)
        {
            var result = base.BuildSearchCriteria(exportDataQuery);

            result.SearchInVariations = exportDataQuery.SearchInVariations;
            result.CatalogIds = exportDataQuery.CatalogIds;
            result.CategoryIds = exportDataQuery.CategoryIds;
            result.SearchInChildren = exportDataQuery.SearchInChildren;
            result.ResponseGroup = exportDataQuery.ResponseGroup;

            return result;
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

        private string BuildResponseGroup()
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

            return result.ToString();
        }
    }
}
