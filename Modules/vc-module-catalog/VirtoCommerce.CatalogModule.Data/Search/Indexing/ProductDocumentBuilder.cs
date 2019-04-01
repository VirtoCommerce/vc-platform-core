using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Extenstions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.Search.Indexing
{
    public class ProductDocumentBuilder : CatalogDocumentBuilder, IIndexDocumentBuilder
    {
        private readonly IItemService _itemService;
        public ProductDocumentBuilder(ISettingsManager settingsManager, IItemService itemService)
            : base(settingsManager)
        {
            _itemService = itemService;
        }

        public virtual async Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
        {
            var products = await GetProducts(documentIds);

            IList<IndexDocument> result = products
                .Select(CreateDocument)
                .Where(doc => doc != null)
                .ToArray();

            return result;
        }


        protected virtual Task<CatalogProduct[]> GetProducts(IList<string> productIds)
        {
            return _itemService.GetByIdsAsync(productIds.ToArray(), null);
        }

        protected virtual IndexDocument CreateDocument(CatalogProduct product)
        {
            var document = new IndexDocument(product.Id);

            document.AddFilterableValue("__type", product.GetType().Name);
            document.AddFilterableValue("__sort", product.Name);

            var statusField = product.IsActive != true || product.MainProductId != null ? "hidden" : "visible";
            IndexIsProperty(document, statusField);
            IndexIsProperty(document, "product");
            IndexIsProperty(document, product.Code);

            document.AddFilterableValue("status", statusField);
            document.AddFilterableAndSearchableValue("code", product.Code);// { IsRetrievable = true, IsFilterable = true, IsCollection = true });
            document.AddFilterableAndSearchableValue("name", product.Name);
            document.AddFilterableValue("startdate", product.StartDate);
            document.AddFilterableValue("enddate", product.EndDate ?? DateTime.MaxValue);
            document.AddFilterableValue("createddate", product.CreatedDate);
            document.AddFilterableValue("lastmodifieddate", product.ModifiedDate ?? DateTime.MaxValue);
            document.AddFilterableValue("modifieddate", product.ModifiedDate ?? DateTime.MaxValue);
            document.AddFilterableValue("priority", product.Priority);
            document.AddFilterableValue("vendor", product.Vendor ?? "");
            document.AddFilterableValue("productType", product.ProductType ?? "");

            // Add priority in virtual categories to search index
            if (product.Links != null)
            {
                foreach (var link in product.Links)
                {
                    document.AddFilterableValue($"priority_{link.CatalogId}_{link.CategoryId}", link.Priority);
                }
            }

            // Add catalogs to search index
            var catalogs = product.Outlines
                .Select(o => o.Items.First().Id)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            document.AddFilterableValues("catalog", catalogs);

            // Add outlines to search index
            var outlineStrings = GetOutlineStrings(product.Outlines);
            document.AddFilterableValues("__outline", outlineStrings);

            // Types of properties which values should be added to the searchable __content field
            var contentPropertyTypes = new[] { PropertyType.Product, PropertyType.Variation };

            // Index custom product properties
            IndexCustomProperties(document, product.Properties, contentPropertyTypes);

            //Index product category properties
            if (product.Category != null)
            {
                IndexCustomProperties(document, product.Category.Properties, contentPropertyTypes);
            }

            //Index catalog properties
            if (product.Catalog != null)
            {
                IndexCustomProperties(document, product.Catalog.Properties, contentPropertyTypes);
            }

            if (product.Variations != null)
            {
                if (product.Variations.Any(c => c.ProductType == "Physical"))
                {
                    document.Add(new IndexDocumentField("type", "physical") { IsRetrievable = true, IsFilterable = true, IsCollection = true });
                    IndexIsProperty(document, "physical");
                }

                if (product.Variations.Any(c => c.ProductType == "Digital"))
                {
                    document.Add(new IndexDocumentField("type", "digital") { IsRetrievable = true, IsFilterable = true, IsCollection = true });
                    IndexIsProperty(document, "digital");
                }

                foreach (var variation in product.Variations)
                {
                    document.Add(new IndexDocumentField("code", variation.Code) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
                    // add the variation code to content
                    document.Add(new IndexDocumentField("__content", variation.Code) { IsRetrievable = true, IsSearchable = true, IsCollection = true });
                    IndexCustomProperties(document, variation.Properties, contentPropertyTypes);
                }
            }

            if (StoreObjectsInIndex)
            {
                // Index serialized product
                document.AddObjectFieldValue(product);
            }

            return document;
        }

        protected virtual void IndexIsProperty(IndexDocument document, string value)
        {
            document.Add(new IndexDocumentField("is", value) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
        }
    }
}
