using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Extensions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogFullExportPagedDataSource : IPagedDataSource
    {
        private readonly CatalogExportPagedDataSourceFactory _catalogDataSourceFactory;
        private readonly CategoryExportPagedDataSourceFactory _categoryDataSourceFactory;
        private readonly PropertyExportPagedDataSourceFactory _propertyDataSourceFactory;
        private readonly PropertyDictionaryItemExportPagedDataSourceFactory _propertyDictionaryItemDataSourceFactory;
        private readonly ProductExportPagedDataSourceFactory _productDataSourceFactory;
        private readonly CatalogFullExportDataQuery _dataQuery;
        private readonly IEnumerable<IPagedDataSource> _dataSources;


        public int CurrentPageNumber { get; protected set; }
        public int PageSize { get; set; } = 50;
        public int? Skip { get => _dataQuery.Skip; set => _dataQuery.Skip = value; }
        public int? Take { get => _dataQuery.Take; set => _dataQuery.Take = value; }
        public IEnumerable<IExportable> Items { get; protected set; }

        public CatalogFullExportPagedDataSource(CatalogExportPagedDataSourceFactory catalogDataSourceFactory,
            CategoryExportPagedDataSourceFactory categoryDataSourceFactory,
            PropertyExportPagedDataSourceFactory propertyDataSourceFactory,
            PropertyDictionaryItemExportPagedDataSourceFactory propertyDictionaryItemDataSourceFactory,
            ProductExportPagedDataSourceFactory productDataSourceFactory,
            CatalogFullExportDataQuery dataQuery)
        {
            _catalogDataSourceFactory = catalogDataSourceFactory;
            _categoryDataSourceFactory = categoryDataSourceFactory;
            _propertyDataSourceFactory = propertyDataSourceFactory;
            _propertyDictionaryItemDataSourceFactory = propertyDictionaryItemDataSourceFactory;
            _productDataSourceFactory = productDataSourceFactory;
            _dataQuery = dataQuery;

            _dataSources = CreateDataSources();
        }

        protected virtual IEnumerable<IPagedDataSource> CreateDataSources()
        {
            var catalogExportDataQuery = AbstractTypeFactory<CatalogExportDataQuery>.TryCreateInstance();
            catalogExportDataQuery.CatalogIds = _dataQuery.CatalogIds;

            var categoryExportDataQuery = AbstractTypeFactory<CategoryExportDataQuery>.TryCreateInstance();
            categoryExportDataQuery.CatalogIds = _dataQuery.CatalogIds;

            var propertyExportDataQuery = AbstractTypeFactory<PropertyExportDataQuery>.TryCreateInstance();
            propertyExportDataQuery.CatalogIds = _dataQuery.CatalogIds;

            var propertyDictionaryItemExportDataQuery = AbstractTypeFactory<PropertyDictionaryItemExportDataQuery>.TryCreateInstance();
            propertyDictionaryItemExportDataQuery.CatalogIds = _dataQuery.CatalogIds;

            var productExportDataQuery = AbstractTypeFactory<ProductExportDataQuery>.TryCreateInstance();
            productExportDataQuery.CatalogIds = _dataQuery.CatalogIds;
            productExportDataQuery.SearchInVariations = true;
            productExportDataQuery.ResponseGroup = (ItemResponseGroup.Full & ~ItemResponseGroup.Variations).ToString();

            return new IPagedDataSource[]
            {
                _catalogDataSourceFactory.Create(catalogExportDataQuery),
                _categoryDataSourceFactory.Create(categoryExportDataQuery),
                _propertyDataSourceFactory.Create(propertyExportDataQuery),
                _propertyDictionaryItemDataSourceFactory.Create(propertyDictionaryItemExportDataQuery),
                _productDataSourceFactory.Create(productExportDataQuery),
            };
        }

        public bool Fetch()
        {
            var skip = Skip ?? CurrentPageNumber * PageSize;
            var take = Take ?? PageSize;

            Items = _dataSources.GetItems(skip, take);
            CurrentPageNumber++;

            return !Items.IsNullOrEmpty();
        }

        public int GetTotalCount() => _dataSources.GetTotalCount();
    }
}
