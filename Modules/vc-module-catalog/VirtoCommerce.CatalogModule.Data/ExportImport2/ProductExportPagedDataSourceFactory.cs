using System;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Assets;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class ProductExportPagedDataSourceFactory : IPagedDataSourceFactory
    {
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IProductSearchService _productSearchService;
        private readonly IItemService _itemService;

        public ProductExportPagedDataSourceFactory(IBlobStorageProvider blobStorageProvider, IProductSearchService productSearchService, IItemService itemService)
        {
            _blobStorageProvider = blobStorageProvider;
            _productSearchService = productSearchService;
            _itemService = itemService;
        }

        public IPagedDataSource Create(ExportDataQuery dataQuery)
        {
            var priceExportDataQuery = dataQuery as ProductExportDataQuery ?? throw new InvalidCastException($"Cannot cast dataQuery to type {typeof(ProductExportDataQuery)}");

            return new ProductExportPagedDataSource(_blobStorageProvider, _itemService, _productSearchService, priceExportDataQuery);

        }
    }
}
