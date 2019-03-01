using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CatalogModule.Web.ExportImport
{
    public sealed class CatalogExportImport
    {
        private readonly ICatalogService _catalogService;
        private readonly ICatalogSearchService _catalogSearchService;
        private readonly ICategoryService _categoryService;
        private readonly IItemService _itemService;
        private readonly IPropertyService _propertyService;
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IAssociationService _associationService;
        private readonly ISettingsManager _settingsManager;
        private readonly JsonSerializer _serializer;
        private readonly IProperyDictionaryItemSearchService _propertyDictionarySearchService;
        private readonly IProperyDictionaryItemService _propertyDictionaryService;

        private int? _batchSize;

        public CatalogExportImport(
            ICatalogSearchService catalogSearchService,
            ICatalogService catalogService,
            ICategoryService categoryService,
            IItemService itemService,
            IPropertyService propertyService,
            IBlobStorageProvider blobStorageProvider,
            IAssociationService associationService,
            ISettingsManager settingsManager,
            IProperyDictionaryItemSearchService propertyDictionarySearchService,
            IProperyDictionaryItemService propertyDictionaryService
            )
        {
            _blobStorageProvider = blobStorageProvider;
            _catalogSearchService = catalogSearchService;
            _catalogService = catalogService;
            _categoryService = categoryService;
            _itemService = itemService;
            _propertyService = propertyService;
            _associationService = associationService;
            _settingsManager = settingsManager;
            _propertyDictionarySearchService = propertyDictionarySearchService;
            _propertyDictionaryService = propertyDictionaryService;

            _serializer = new JsonSerializer
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        private int BatchSize
        {
            get
            {
                if (_batchSize == null)
                {
                    _batchSize = _settingsManager.GetValue("Catalog.ExportImport.PageSize", 50);
                }

                return (int)_batchSize;
            }
        }

        #region Export/Import methods
        public void DoExport(Stream outStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var progressInfo = new ExportImportProgressInfo { Description = "loading data…" };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream, Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                // Properties and Property dictionary items should preceed Catalogs and Categories for proper import
                ExportProperties(writer, _serializer, progressInfo, progressCallback);
                ExportPropertiesDictionaryItems(writer, _serializer, progressInfo, progressCallback);
                ExportCatalogs(writer, _serializer, progressInfo, progressCallback);
                ExportCategories(writer, _serializer, manifest, progressInfo, progressCallback);
                ExportProducts(writer, _serializer, manifest, progressInfo, progressCallback);

                writer.WriteEndObject();
                writer.Flush();
            }
        }

        public void DoImport(Stream stream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var progressInfo = new ExportImportProgressInfo();
            var productsTotalCount = 0;
            var propDictItemTotalCount = 0;
            var propertiesWithForeignKeys = new List<Property>();
            using (var streamReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value.ToString() == "Catalogs")
                        {
                            reader.Read();
                            var catalogs = _serializer.Deserialize<Catalog[]>(reader);
                            progressInfo.Description = $"{ catalogs.Count() } catalogs are importing…";
                            progressCallback(progressInfo);
                            _catalogService.Update(catalogs);
                        }
                        else if (reader.Value.ToString() == "Categories")
                        {
                            reader.Read();
                            var categories = _serializer.Deserialize<Category[]>(reader);
                            progressInfo.Description = $"{ categories.Count() } categories are importing…";
                            progressCallback(progressInfo);
                            _categoryService.Update(categories);
                            if (manifest.HandleBinaryData)
                            {
                                ImportImages(categories, progressInfo);
                            }
                        }
                        else if (reader.Value.ToString() == "Properties")
                        {
                            reader.Read();
                            var properties = _serializer.Deserialize<Property[]>(reader);
                            foreach (var property in properties)
                            {
                                if (property.CategoryId != null || property.CatalogId != null)
                                {
                                    propertiesWithForeignKeys.Add(property.Clone() as Property);
                                    //Need to reset property foreign keys to prevent FK violation during  inserting into database 
                                    property.CategoryId = null;
                                    property.CatalogId = null;
                                }
                            }
                            progressInfo.Description = $"{ properties.Count() } properties are importing…";
                            progressCallback(progressInfo);
                            _propertyService.Update(properties);
                        }
                        else if (reader.Value.ToString() == "ProductsTotalCount")
                        {
                            productsTotalCount = reader.ReadAsInt32() ?? 0;
                        }
                        else if (reader.Value.ToString() == "PropertyDictionaryItemsTotalCount")
                        {
                            propDictItemTotalCount = reader.ReadAsInt32() ?? 0;
                        }
                        else if (reader.Value.ToString() == "PropertyDictionaryItems")
                        {
                            reader.Read();
                            if (reader.TokenType == JsonToken.StartArray)
                            {
                                reader.Read();

                                var propDictItems = new List<PropertyDictionaryItem>();
                                var propDictItemsCount = 0;
                                //Import property dcitonary items
                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    var propDictItem = AbstractTypeFactory<PropertyDictionaryItem>.TryCreateInstance();
                                    propDictItem = _serializer.Deserialize(reader, propDictItem.GetType()) as PropertyDictionaryItem;
                                    propDictItems.Add(propDictItem);
                                    propDictItemsCount++;
                                    reader.Read();
                                    if (propDictItemsCount % BatchSize == 0 || reader.TokenType == JsonToken.EndArray)
                                    {
                                        _propertyDictionaryService.SaveChanges(propDictItems.ToArray());
                                        propDictItems.Clear();
                                        if (propDictItemsCount > 0)
                                        {
                                            progressInfo.Description = $"{ propDictItemsCount } of { propDictItemTotalCount } dictionary items have been imported";
                                        }
                                        else
                                        {
                                            progressInfo.Description = $"{ propDictItemsCount } dictionary items have been imported";
                                        }
                                        progressCallback(progressInfo);
                                    }
                                }
                            }
                        }
                        else if (reader.Value.ToString() == "Products")
                        {
                            reader.Read();

                            if (reader.TokenType == JsonToken.StartArray)
                            {
                                reader.Read();

                                var associationBackupMap = new Dictionary<string, ICollection<ProductAssociation>>();
                                var products = new List<CatalogProduct>();
                                var productsCount = 0;
                                //Import products
                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    var product = AbstractTypeFactory<CatalogProduct>.TryCreateInstance();
                                    product = _serializer.Deserialize(reader, product.GetType()) as CatalogProduct;
                                    //Do not save associations withing product to prevent dependency conflicts in db
                                    //we will save separateley after product import
                                    if (!product.Associations.IsNullOrEmpty())
                                    {
                                        associationBackupMap[product.Id] = product.Associations;
                                    }
                                    product.Associations = null;
                                    products.Add(product);
                                    productsCount++;
                                    reader.Read();
                                    if (productsCount % BatchSize == 0 || reader.TokenType == JsonToken.EndArray)
                                    {
                                        _itemService.Update(products.ToArray());
                                        if (manifest.HandleBinaryData)
                                        {
                                            ImportImages(products.ToArray(), progressInfo);
                                        }

                                        products.Clear();
                                        if (productsTotalCount > 0)
                                        {
                                            progressInfo.Description = $"{ productsCount } of { productsTotalCount } products have been imported";
                                        }
                                        else
                                        {
                                            progressInfo.Description = $"{ productsCount } products have been imported";
                                        }
                                        progressCallback(progressInfo);
                                    }
                                }
                                //Import products associations separately to avoid DB constrain violation
                                var totalProductsWithAssociationsCount = associationBackupMap.Count;
                                progressInfo.Description = $"{ totalProductsWithAssociationsCount } products associations are importing…";
                                progressCallback(progressInfo);
                                for (var i = 0; i < totalProductsWithAssociationsCount; i += BatchSize)
                                {
                                    var fakeProducts = new List<CatalogProduct>();
                                    foreach (var pair in associationBackupMap.Skip(i).Take(BatchSize))
                                    {
                                        var fakeProduct = AbstractTypeFactory<CatalogProduct>.TryCreateInstance();
                                        fakeProduct.Id = pair.Key;
                                        fakeProduct.Associations = pair.Value;
                                        fakeProducts.Add(fakeProduct);
                                    }
                                    _associationService.SaveChanges(fakeProducts.ToArray());
                                    progressInfo.Description = $"{ Math.Min(totalProductsWithAssociationsCount, i + BatchSize) } of { totalProductsWithAssociationsCount } products associations imported";
                                    progressCallback(progressInfo);
                                }
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
                for (var i = 0; i < totalCount; i += BatchSize)
                {
                    _propertyService.Update(propertiesWithForeignKeys.Skip(i).Take(BatchSize).ToArray());
                    progressInfo.Description = $"{ Math.Min(totalCount, i + BatchSize) } of { totalCount } property associations updated.";
                    progressCallback(progressInfo);
                }
            }
        }
        #endregion

        private void ExportCatalogs(JsonTextWriter writer, JsonSerializer serializer, ExportImportProgressInfo progressInfo, Action<ExportImportProgressInfo> progressCallback)
        {
            //Catalogs
            progressInfo.Description = "Catalogs are exporting…";
            progressCallback(progressInfo);

            var catalogs = _catalogService.GetCatalogsList().ToArray();

            writer.WritePropertyName("Catalogs");
            writer.WriteStartArray();
            //Reset some props to decrease resulting json size
            foreach (var catalog in catalogs)
            {
                ResetRedundantReferences(catalog);
                serializer.Serialize(writer, catalog);
            }
            writer.WriteEndArray();


            progressInfo.Description = $"{ catalogs.Count() } catalogs have been exported";
            progressCallback(progressInfo);
        }

        private void ExportCategories(JsonTextWriter writer, JsonSerializer serializer, PlatformExportManifest manifest, ExportImportProgressInfo progressInfo, Action<ExportImportProgressInfo> progressCallback)
        {
            //Categories
            progressInfo.Description = "Categories are exporting…";
            progressCallback(progressInfo);

            var categorySearchCriteria = new SearchCriteria { WithHidden = true, Skip = 0, Take = 0, ResponseGroup = SearchResponseGroup.WithCategories };
            var categoriesSearchResult = _catalogSearchService.Search(categorySearchCriteria);
            var categories = _categoryService.GetByIds(categoriesSearchResult.Categories.Select(x => x.Id).ToArray(), CategoryResponseGroup.Full);
            if (manifest.HandleBinaryData)
            {
                LoadImages(categories, progressInfo);
            }

            writer.WritePropertyName("Categories");
            writer.WriteStartArray();
            //reset some properties to decrease resulting JSON size
            foreach (var category in categories)
            {
                ResetRedundantReferences(category);
                serializer.Serialize(writer, category);
            }
            writer.WriteEndArray();

            progressInfo.Description = $"{ categoriesSearchResult.Categories.Count } categories have been exported";
            progressCallback(progressInfo);
        }

        private void ExportProperties(JsonTextWriter writer, JsonSerializer serializer, ExportImportProgressInfo progressInfo, Action<ExportImportProgressInfo> progressCallback)
        {
            //Properties
            progressInfo.Description = "Properties exporting…";
            progressCallback(progressInfo);

            var properties = _propertyService.GetAllProperties();
            writer.WritePropertyName("Properties");
            writer.WriteStartArray();
            //Load property dictionary values and reset some props to decrease size of the resulting json 
            foreach (var property in properties)
            {
                ResetRedundantReferences(property);
                serializer.Serialize(writer, property);
            }
            writer.WriteEndArray();
            progressInfo.Description = $"{ properties.Count() } properties have been exported";
            progressCallback(progressInfo);
        }

        private void ExportPropertiesDictionaryItems(JsonTextWriter writer, JsonSerializer serializer, ExportImportProgressInfo progressInfo, Action<ExportImportProgressInfo> progressCallback)
        {
            progressInfo.Description = "The dictionary items are exporting…";
            progressCallback(progressInfo);

            var criteria = new PropertyDictionaryItemSearchCriteria { Take = 0, Skip = 0 };
            var totalCount = _propertyDictionarySearchService.Search(criteria).TotalCount;
            writer.WritePropertyName("PropertyDictionaryItemsTotalCount");
            writer.WriteValue(totalCount);

            writer.WritePropertyName("PropertyDictionaryItems");
            writer.WriteStartArray();
            for (var i = 0; i < totalCount; i += BatchSize)
            {
                var searchResponse = _propertyDictionarySearchService.Search(new PropertyDictionaryItemSearchCriteria { Take = BatchSize, Skip = i });
                foreach (var dictItem in searchResponse.Results)
                {
                    serializer.Serialize(writer, dictItem);
                }
                writer.Flush();
                progressInfo.Description = $"{ Math.Min(totalCount, i + BatchSize) } of { totalCount } dictionary items have been exported";
                progressCallback(progressInfo);
            }
            writer.WriteEndArray();
        }

        private void ExportProducts(JsonTextWriter writer, JsonSerializer serializer, PlatformExportManifest manifest, ExportImportProgressInfo progressInfo, Action<ExportImportProgressInfo> progressCallback)
        {
            //Products
            progressInfo.Description = "Products are exporting…";
            progressCallback(progressInfo);

            var productSearchCriteria = new SearchCriteria { WithHidden = true, Take = 0, Skip = 0, ResponseGroup = SearchResponseGroup.WithProducts };
            var totalProductCount = _catalogSearchService.Search(productSearchCriteria).ProductsTotalCount;
            writer.WritePropertyName("ProductsTotalCount");
            writer.WriteValue(totalProductCount);

            writer.WritePropertyName("Products");
            writer.WriteStartArray();
            for (var i = 0; i < totalProductCount; i += BatchSize)
            {
                var searchResponse = _catalogSearchService.Search(new SearchCriteria { WithHidden = true, Take = BatchSize, Skip = i, ResponseGroup = SearchResponseGroup.WithProducts });

                var products = _itemService.GetByIds(searchResponse.Products.Select(x => x.Id).ToArray(), ItemResponseGroup.ItemLarge);
                if (manifest.HandleBinaryData)
                {
                    LoadImages(products, progressInfo);
                }
                foreach (var product in products)
                {
                    ResetRedundantReferences(product);
                    serializer.Serialize(writer, product);
                }
                writer.Flush();
                progressInfo.Description = $"{ Math.Min(totalProductCount, i + BatchSize) } of { totalProductCount } products have been exported";
                progressCallback(progressInfo);
            }
            writer.WriteEndArray();
        }

        //Remove redundant references to reduce resulting JSON size
        private static void ResetRedundantReferences(object entity)
        {
            var product = entity as CatalogProduct;
            var category = entity as Category;
            var catalog = entity as Catalog;
            var asscociation = entity as ProductAssociation;
            var property = entity as Property;
            var propertyValue = entity as PropertyValue;

            if (propertyValue != null)
            {
                propertyValue.Property = null;
            }

            if (asscociation != null)
            {
                asscociation.AssociatedObject = null;
            }

            if (catalog != null)
            {
                catalog.Properties = null;
                foreach (var lang in catalog.Languages)
                {
                    lang.Catalog = null;
                }
            }

            if (category != null)
            {
                category.Catalog = null;
                category.Properties = null;
                category.Children = null;
                category.Parents = null;
                category.Outlines = null;
                if (category.PropertyValues != null)
                {
                    foreach (var propvalue in category.PropertyValues)
                    {
                        ResetRedundantReferences(propvalue);
                    }
                }
            }

            if (property != null)
            {
                property.Catalog = null;
                property.Category = null;
            }

            if (product != null)
            {
                product.Catalog = null;
                product.Category = null;
                product.Properties = null;
                product.MainProduct = null;
                product.Outlines = null;
                product.ReferencedAssociations = null;
                if (product.PropertyValues != null)
                {
                    foreach (var propvalue in product.PropertyValues)
                    {
                        ResetRedundantReferences(propvalue);
                    }
                }
                if (product.Associations != null)
                {
                    foreach (var association in product.Associations)
                    {
                        ResetRedundantReferences(association);
                    }
                }
                if (product.Variations != null)
                {
                    foreach (var variation in product.Variations)
                    {
                        ResetRedundantReferences(variation);
                    }
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
                    //do not save images with external url
                    if (image.Url != null && !image.Url.IsAbsoluteUrl())
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
