using System;
using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogExportPagedDataSourceFactory : ICatalogExportPagedDataSourceFactory
    {
        private readonly IPropertySearchService _propertySearchService;
        private readonly IPropertyDictionaryItemSearchService _propertyDictionaryItemSearchService;
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IProductSearchService _productSearchService;
        private readonly IItemService _itemService;
        private readonly ICategorySearchService _categorySearchService;
        private readonly ICatalogSearchService _catalogSearchService;

        public CatalogExportPagedDataSourceFactory(
            IPropertySearchService propertySearchService
            , IPropertyDictionaryItemSearchService propertyDictionaryItemSearchService
            , IBlobStorageProvider blobStorageProvider
            , IProductSearchService productSearchService
            , IItemService itemService
            , ICategorySearchService categorySearchService
            , ICatalogSearchService catalogSearchService)
        {
            _propertySearchService = propertySearchService;
            _propertyDictionaryItemSearchService = propertyDictionaryItemSearchService;
            _blobStorageProvider = blobStorageProvider;
            _productSearchService = productSearchService;
            _itemService = itemService;
            _categorySearchService = categorySearchService;
            _catalogSearchService = catalogSearchService;
        }

        public virtual IPagedDataSource Create(ExportDataQuery dataQuery)
        {
            IPagedDataSource result = null;

            if (dataQuery is PropertyExportDataQuery propertyExportQuery)
            {
                result = new PropertyExportPagedDataSource(_propertySearchService, propertyExportQuery);
            }
            else if (dataQuery is PropertyDictionaryItemExportDataQuery propDictExportQuery)
            {
                result = new PropertyDictionaryItemExportPagedDataSource(_propertyDictionaryItemSearchService, propDictExportQuery);
            }
            else if (dataQuery is ProductFullExportDataQuery productFullExportQuery)
            {
                result = new ProductExportPagedDataSource(_blobStorageProvider, _itemService, _productSearchService, productFullExportQuery.ToProductExportDataQuery());
            }
            else if (dataQuery is CategoryExportDataQuery categoryExportQuery)
            {
                result = new CategoryExportPagedDataSource(_categorySearchService, categoryExportQuery);
            }
            else if (dataQuery is CatalogExportDataQuery catalogExportQuery)
            {
                result = new CatalogExportPagedDataSource(_catalogSearchService, catalogExportQuery);
            }
            else if (dataQuery is ProductExportDataQuery productExportQuery)
            {
                result = new ProductExportPagedDataSource(_blobStorageProvider, _itemService, _productSearchService, productExportQuery);
            }
            else if (dataQuery is CatalogFullExportDataQuery catalogFullExportQuery)
            {
                result = new CatalogFullExportPagedDataSource(this, catalogFullExportQuery);
            }

            if (result == null)
            {
                throw new ArgumentException($"Unsupported export query type: {dataQuery.GetType().Name}");
            }
            return result;
        }

        public virtual IEnumerable<IPagedDataSource> GetAllFullExportPagedDataSources(CatalogFullExportDataQuery query)
        {
            yield return Create(AbstractTypeFactory<CatalogExportDataQuery>.TryCreateInstance().FromOther(query));
            yield return Create(AbstractTypeFactory<CategoryExportDataQuery>.TryCreateInstance().FromOther(query));
            yield return Create(AbstractTypeFactory<ProductFullExportDataQuery>.TryCreateInstance().FromOther(query));
            yield return Create(AbstractTypeFactory<PropertyExportDataQuery>.TryCreateInstance().FromOther(query));
            yield return Create(AbstractTypeFactory<PropertyDictionaryItemExportDataQuery>.TryCreateInstance().FromOther(query));
        }
    }
}
