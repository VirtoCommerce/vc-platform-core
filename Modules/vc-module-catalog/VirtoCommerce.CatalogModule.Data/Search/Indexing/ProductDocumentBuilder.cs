using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Web.Converters;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CatalogModule.Data.Search.Indexing
{
    public class ProductDocumentBuilder : CatalogDocumentBuilder, IIndexDocumentBuilder
    {
        private readonly IItemService _itemService;
        private readonly IBlobUrlResolver _blobUrlResolver;
        public ProductDocumentBuilder(ISettingsManager settingsManager, IItemService itemService, IBlobUrlResolver blobUrlResolver)
            : base(settingsManager)
        {
            _itemService = itemService;
            _blobUrlResolver = blobUrlResolver;
        }

        public virtual Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
        {
            var products = GetProducts(documentIds);

            IList<IndexDocument> result = products
                .Select(CreateDocument)
                .Where(doc => doc != null)
                .ToArray();

            return Task.FromResult(result);
        }


        protected virtual IList<CatalogProduct> GetProducts(IList<string> productIds)
        {
            return _itemService.GetByIds(productIds.ToArray(), ItemResponseGroup.ItemLarge);
        }

        protected virtual IndexDocument CreateDocument(CatalogProduct product)
        {
            var document = new IndexDocument(product.Id);

            document.Add(new IndexDocumentField("__type", product.GetType().Name) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("__sort", product.Name) { IsRetrievable = true, IsFilterable = true });

            var statusField = product.IsActive != true || product.MainProductId != null ? "hidden" : "visible";
            IndexIsProperty(document, statusField);
            IndexIsProperty(document, "product");
            IndexIsProperty(document, product.Code);

            document.Add(new IndexDocumentField("status", statusField) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("code", product.Code) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
            document.Add(new IndexDocumentField("name", product.Name) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("startdate", product.StartDate) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("enddate", product.EndDate ?? DateTime.MaxValue) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("createddate", product.CreatedDate) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("lastmodifieddate", product.ModifiedDate ?? DateTime.MaxValue) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("modifieddate", product.ModifiedDate ?? DateTime.MaxValue) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("priority", product.Priority) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("vendor", product.Vendor ?? "") { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("productType", product.ProductType ?? "") { IsRetrievable = true, IsFilterable = true });

            // Add priority in virtual categories to search index
            if (product.Links != null)
            {
                foreach (var link in product.Links)
                {
                    document.Add(new IndexDocumentField($"priority_{link.CatalogId}_{link.CategoryId}", link.Priority) { IsRetrievable = true, IsFilterable = true });
                }
            }

            // Add catalogs to search index
            var catalogs = product.Outlines
                .Select(o => o.Items.First().Id)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            foreach (var catalogId in catalogs)
            {
                document.Add(new IndexDocumentField("catalog", catalogId.ToLowerInvariant()) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
            }

            // Add outlines to search index
            var outlineStrings = GetOutlineStrings(product.Outlines);
            foreach (var outline in outlineStrings)
            {
                document.Add(new IndexDocumentField("__outline", outline.ToLowerInvariant()) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
            }

            // Types of properties which values should be added to the searchable __content field
            var contentPropertyTypes = new[] { PropertyType.Product, PropertyType.Variation };

            // Index custom product properties
            IndexCustomProperties(document, product.Properties, product.PropertyValues, contentPropertyTypes);

            //Index product category properties
            if (product.Category != null)
            {
                IndexCustomProperties(document, product.Category.Properties, product.Category.PropertyValues, contentPropertyTypes);
            }

            //Index catalog properties
            if (product.Catalog != null)
            {
                IndexCustomProperties(document, product.Catalog.Properties, product.Catalog.PropertyValues, contentPropertyTypes);
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
                    IndexCustomProperties(document, variation.Properties, variation.PropertyValues, contentPropertyTypes);
                }
            }

            // add to content
            document.Add(new IndexDocumentField("__content", product.Name) { IsRetrievable = true, IsSearchable = true, IsCollection = true });
            document.Add(new IndexDocumentField("__content", product.Code) { IsRetrievable = true, IsSearchable = true, IsCollection = true });

            if (StoreObjectsInIndex)
            {
                // Index serialized product
                var itemDto = product.ToWebModel(_blobUrlResolver);
                document.AddObjectFieldValue(itemDto);
            }

            return document;
        }

        protected virtual void IndexIsProperty(IndexDocument document, string value)
        {
            document.Add(new IndexDocumentField("is", value) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
        }
    }
}
