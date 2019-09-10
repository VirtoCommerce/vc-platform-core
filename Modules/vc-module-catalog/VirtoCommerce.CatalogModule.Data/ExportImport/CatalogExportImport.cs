using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Data.ExportImport;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogExportImport
    {
        private readonly ICatalogService _catalogService;
        private readonly ICatalogSearchService _catalogSearchService;
        private readonly IProductSearchService _productSearchService;
        private readonly ICategorySearchService _categorySearchService;
        private readonly ICategoryService _categoryService;
        private readonly IItemService _itemService;
        private readonly IPropertyService _propertyService;
        private readonly IPropertySearchService _propertySearchService;
        private readonly IPropertyDictionaryItemSearchService _propertyDictionarySearchService;
        private readonly IPropertyDictionaryItemService _propertyDictionaryService;
        private readonly JsonSerializer _jsonSerializer;
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IAssociationService _associationService;

        private int _batchSize = 50;

        public CatalogExportImport(ICatalogService catalogService, ICatalogSearchService catalogSearchService, IProductSearchService productSearchService, ICategorySearchService categorySearchService, ICategoryService categoryService,
                                  IItemService itemService, IPropertyService propertyService, IPropertySearchService propertySearchService, IPropertyDictionaryItemSearchService propertyDictionarySearchService,
                                  IPropertyDictionaryItemService propertyDictionaryService, JsonSerializer jsonSerializer, IBlobStorageProvider blobStorageProvider, IAssociationService associationService)
        {
            _catalogService = catalogService;
            _productSearchService = productSearchService;
            _categorySearchService = categorySearchService;
            _categoryService = categoryService;
            _itemService = itemService;
            _propertyService = propertyService;
            _propertySearchService = propertySearchService;
            _propertyDictionarySearchService = propertyDictionarySearchService;
            _propertyDictionaryService = propertyDictionaryService;
            _jsonSerializer = jsonSerializer;
            _blobStorageProvider = blobStorageProvider;
            _associationService = associationService;
            _catalogSearchService = catalogSearchService;
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

                #region Export catalogs
                progressInfo.Description = "Catalogs exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Catalogs");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                    (GenericSearchResult<Catalog>)await _catalogSearchService.SearchCatalogsAsync(new CatalogSearchCriteria { Skip = skip, Take = take })             
                , (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } catalogs have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);
                #endregion

                #region Export categories
                progressInfo.Description = "Categories exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Categories");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    var searchResult = await _categorySearchService.SearchCategoriesAsync(new CategorySearchCriteria { Skip = skip, Take = take });
                    if (options.HandleBinaryData)
                    {
                        LoadImages(searchResult.Results.OfType<IHasImages>().ToArray(), progressInfo);
                    }

                    return (GenericSearchResult<Category>)searchResult;
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } Categories have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);
                #endregion

                #region Export properties
                progressInfo.Description = "Properties exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Properties");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                    (GenericSearchResult<Property>)await _propertySearchService.SearchPropertiesAsync(new PropertySearchCriteria { Skip = skip, Take = take })
                , (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } properties have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);
                #endregion

                #region Export propertyDictionaryItems 
                progressInfo.Description = "PropertyDictionaryItems exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("PropertyDictionaryItems");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                    (GenericSearchResult<PropertyDictionaryItem>)await _propertyDictionarySearchService.SearchAsync(new PropertyDictionaryItemSearchCriteria { Skip = skip, Take = take })
                , (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } property dictionary items have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);
                #endregion

                #region Export products
                progressInfo.Description = "Products exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Products");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    var searchResult = await _productSearchService.SearchProductsAsync(new ProductSearchCriteria { Skip = skip, Take = take, ResponseGroup = (ItemResponseGroup.Full & ~ItemResponseGroup.Variations).ToString() });
                    if (options.HandleBinaryData)
                    {
                        LoadImages(searchResult.Results.OfType<IHasImages>().ToArray(), progressInfo);
                    }
                    return (GenericSearchResult<CatalogProduct>)searchResult;
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } Products have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);
                #endregion

                await writer.WriteEndObjectAsync();
                await writer.FlushAsync();
            }
        }

        public async Task DoImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();
            var propertiesWithForeignKeys = new List<Property>();

            using (var streamReader = new StreamReader(inputStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value.ToString() == "Catalogs")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<Catalog>(_jsonSerializer, _batchSize, async (items) =>
                            {
                                await _catalogService.SaveChangesAsync(items.ToArray());

                            }, processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } catalogs have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);

                        }
                        else if (reader.Value.ToString() == "Categories")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<Category>(_jsonSerializer, _batchSize, async (items) =>
                            {
                                var itemsArray = items.ToArray();
                                await _categoryService.SaveChangesAsync(itemsArray);
                                //if (options.HandleBinaryData)
                                {
                                    ImportImages(itemsArray.OfType<IHasImages>().ToArray(), progressInfo);
                                }

                            }, processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } categories have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "Properties")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<Property>(_jsonSerializer, _batchSize, async (items) =>
                            {
                                var itemsArray = items.ToArray();
                                foreach (var property in itemsArray)
                                {
                                    if (property.CategoryId != null || property.CatalogId != null)
                                    {
                                        propertiesWithForeignKeys.Add(property.Clone() as Property);
                                        //Need to reset property foreign keys to prevent FK violation during  inserting into database 
                                        property.CategoryId = null;
                                        property.CatalogId = null;
                                    }
                                }
                                await _propertyService.SaveChangesAsync(itemsArray);
                            }, processedCount =>
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
                            var associationBackupMap = new Dictionary<string, IList<ProductAssociation>>();
                            var products = new List<CatalogProduct>();

                            await reader.DeserializeJsonArrayWithPagingAsync<CatalogProduct>(_jsonSerializer, _batchSize, async (items) =>
                            {
                                var itemsArray = items.ToArray();
                                foreach (var product in itemsArray)
                                {
                                    //Do not save associations withing product to prevent dependency conflicts in db
                                    //we will save separateley after product import
                                    if (!product.Associations.IsNullOrEmpty())
                                    {
                                        associationBackupMap[product.Id] = product.Associations;
                                    }

                                    product.Associations = null;
                                    products.Add(product);
                                }
                                await _itemService.SaveChangesAsync(products.ToArray());
                                if (options != null && options.HandleBinaryData)
                                {
                                    ImportImages(itemsArray.OfType<IHasImages>().ToArray(), progressInfo);
                                }

                                products.Clear();
                            }, processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } products have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);

                            //Import products associations separately to avoid DB constrain violation
                            var totalProductsWithAssociationsCount = associationBackupMap.Count;
                            for (var i = 0; i < totalProductsWithAssociationsCount; i += _batchSize)
                            {
                                var fakeProducts = new List<CatalogProduct>();
                                foreach (var pair in associationBackupMap.Skip(i).Take(_batchSize))
                                {
                                    var fakeProduct = AbstractTypeFactory<CatalogProduct>.TryCreateInstance();
                                    fakeProduct.Id = pair.Key;
                                    fakeProduct.Associations = pair.Value;
                                    fakeProducts.Add(fakeProduct);
                                }

                                await _associationService.SaveChangesAsync(fakeProducts.OfType<IHasAssociations>().ToArray());
                                progressInfo.Description = $"{ Math.Min(totalProductsWithAssociationsCount, i + _batchSize) } of { totalProductsWithAssociationsCount } products associations imported";
                                progressCallback(progressInfo);
                            }
                        }
                    }
                }
            }

            //Update property associations after all required data are saved (Catalogs and Categories)
            if (propertiesWithForeignKeys.Count > 0)
            {
                progressInfo.Description = $"Updating {propertiesWithForeignKeys.Count} property associations…";
                progressCallback(progressInfo);

                var totalCount = propertiesWithForeignKeys.Count;
                for (var i = 0; i < totalCount; i += _batchSize)
                {
                    await _propertyService.SaveChangesAsync(propertiesWithForeignKeys.Skip(i).Take(_batchSize).ToArray());
                    progressInfo.Description = $"{ Math.Min(totalCount, i + _batchSize) } of { totalCount } property associations updated.";
                    progressCallback(progressInfo);
                }
            }
        }

        private void LoadImages(IHasImages[] haveImagesObjects, ExportImportProgressInfo progressInfo)
        {
            var allImages = haveImagesObjects.SelectMany(x => x.GetFlatObjectsListWithInterface<IHasImages>())
                                             .SelectMany(x => x.Images).ToArray();
            foreach (var image in allImages)
            {
                try
                {
                    using (var stream = _blobStorageProvider.OpenRead(image.Url))
                    {
                        image.BinaryData = stream.ReadFully();
                    }
                }
                catch (Exception ex)
                {
                    progressInfo.Errors.Add(ex.Message);
                }
            }
        }

        private void ImportImages(IHasImages[] haveImagesObjects, ExportImportProgressInfo progressInfo)
        {

            var allImages = haveImagesObjects.SelectMany(x => x.GetFlatObjectsListWithInterface<IHasImages>())
                                       .SelectMany(x => x.Images).ToArray();
            foreach (var image in allImages.Where(x => x.BinaryData != null))
            {
                try
                {
                    var url = image.Url != null && !image.Url.IsAbsoluteUrl() ? image.Url : image.RelativeUrl;
                    //do not save images with external url
                    if (!string.IsNullOrEmpty(url))
                    {
                        using (var sourceStream = new MemoryStream(image.BinaryData))
                        using (var targetStream = _blobStorageProvider.OpenWrite(image.Url))
                        {
                            sourceStream.CopyTo(targetStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    progressInfo.Errors.Add(ex.Message);
                }
            }
        }
    }
}
