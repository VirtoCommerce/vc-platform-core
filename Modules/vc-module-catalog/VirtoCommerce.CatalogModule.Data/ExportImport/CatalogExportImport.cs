using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Data.ExportImport;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogExportImport
    {
        private readonly ICatalogService _catalogService;
        private readonly ICatalogSearchService _catalogSearchService;
        private readonly ICategoryService _categoryService;
        private readonly ICategorySearchService _categorySearchService;
        private readonly IItemService _itemService;
        private readonly IPropertyService _propertyService;
        private readonly IProductSearchService _productSearchService;
        private readonly IProperyDictionaryItemSearchService _propertyDictionarySearchService;
        private readonly IProperyDictionaryItemService _propertyDictionaryService;
        private readonly JsonSerializer _jsonSerializer;

        private int _batchSize = 50;

        public CatalogExportImport(ICatalogService catalogService, ICatalogSearchService catalogSearchService, ICategoryService categoryService, IItemService itemService, IPropertyService propertyService, IProperyDictionaryItemSearchService propertyDictionarySearchService, IProperyDictionaryItemService propertyDictionaryService, IProductSearchService productSearchService, ICategorySearchService categorySearchService, JsonSerializer jsonSerializer)
        {
            _catalogService = catalogService;
            _catalogSearchService = catalogSearchService;
            _categoryService = categoryService;
            _itemService = itemService;
            _propertyService = propertyService;
            _propertyDictionarySearchService = propertyDictionarySearchService;
            _propertyDictionaryService = propertyDictionaryService;
            _productSearchService = productSearchService;
            _categorySearchService = categorySearchService;
            _jsonSerializer = jsonSerializer;
        }

        public async Task DoExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream))
            using (var writer = new JsonTextWriter(sw))
            {
                await writer.WriteStartObjectAsync();

                progressInfo.Description = "Properties exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Properties");
                await writer.WriteStartArrayAsync();
                var properties = await _propertyService.GetAllPropertiesAsync();

                var propertiesCount = properties.Length;
                for (var i = 0; i < propertiesCount; i += _batchSize)
                {
                    var propertyBatch = properties.Skip(i).Take(_batchSize);
                    foreach (var data in propertyBatch)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        _jsonSerializer.Serialize(writer, data);
                    }
                    await writer.FlushAsync();
                    progressInfo.Description = $"{ Math.Min(propertiesCount, i + _batchSize) } of { propertiesCount } properties have been exported";
                    progressCallback(progressInfo);
                }
                await writer.WriteEndArrayAsync();

                progressInfo.Description = "PropertyDictionaryItems exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("PropertyDictionaryItems");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, (skip, take) => _propertyDictionarySearchService.SearchAsync(new PropertyDictionaryItemSearchCriteria { Skip = skip, Take = take }), (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } property dictionary items have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Catalogs exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Catalogs");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    var searchResult = await _catalogSearchService.SearchAsync(new CatalogListEntrySearchCriteria { Skip = skip, Take = take });
                    return new GenericSearchResult<Catalog> { Results = searchResult.Catalogs, TotalCount = searchResult.Catalogs.Count };
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } catalogs have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Categories exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Categories");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    var searchResult = await _categorySearchService.SearchAsync(new CategorySearchCriteria { Skip = skip, Take = take });
                    return new GenericSearchResult<Category> { Results = searchResult.Items, TotalCount = (int)searchResult.TotalCount };
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } Categories have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Products exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Products");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    var searchResult = await _productSearchService.SearchAsync(new ProductSearchCriteria { Skip = skip, Take = take });
                    return new GenericSearchResult<CatalogProduct> { Results = searchResult.Items, TotalCount = (int)searchResult.TotalCount };
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } Products have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                await writer.WriteEndObjectAsync();
                await writer.FlushAsync();
            }
        }

        public async Task DoImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();

            using (var streamReader = new StreamReader(inputStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value.ToString() == "Catalogs")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<Catalog>(_jsonSerializer, _batchSize, items => _catalogService.SaveChangesAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } catalogs have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);

                        }
                        else if (reader.Value.ToString() == "Categories")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<Category>(_jsonSerializer, _batchSize, items => _categoryService.SaveChangesAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } categories have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "Properties")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<Property>(_jsonSerializer, _batchSize, items => _propertyService.SaveChangesAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } properties have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "PropertyDictionaryItems")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<PropertyDictionaryItem>(_jsonSerializer, _batchSize, items => _propertyDictionaryService.SaveChangesAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } property dictionary items have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "Products")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<CatalogProduct>(_jsonSerializer, _batchSize, items => _itemService.SaveChangesAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } products have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}
