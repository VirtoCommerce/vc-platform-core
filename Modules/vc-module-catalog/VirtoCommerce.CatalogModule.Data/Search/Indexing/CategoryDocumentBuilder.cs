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
    public class CategoryDocumentBuilder : CatalogDocumentBuilder, IIndexDocumentBuilder
    {
        private readonly ICategoryService _categoryService;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public CategoryDocumentBuilder(ISettingsManager settingsManager, ICategoryService categoryService, IBlobUrlResolver blobUrlResolver)
            : base(settingsManager)
        {
            _categoryService = categoryService;
            _blobUrlResolver = blobUrlResolver;
        }

        public virtual Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
        {
            var categories = GetCategories(documentIds);

            IList<IndexDocument> result = categories
                .Select(CreateDocument)
                .Where(doc => doc != null)
                .ToArray();

            return Task.FromResult(result);
        }


        protected virtual IList<Category> GetCategories(IList<string> categoryIds)
        {
            return _categoryService.GetByIds(categoryIds.ToArray(), CategoryResponseGroup.WithProperties | CategoryResponseGroup.WithOutlines | CategoryResponseGroup.WithImages | CategoryResponseGroup.WithSeo | CategoryResponseGroup.WithLinks);
        }

        protected virtual IndexDocument CreateDocument(Category category)
        {
            var document = new IndexDocument(category.Id);

            document.Add(new IndexDocumentField("__key", category.Id.ToLowerInvariant()) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("__type", category.GetType().Name) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("__sort", category.Name) { IsRetrievable = true, IsFilterable = true });

            var statusField = category.IsActive != true ? "hidden" : "visible";
            IndexIsProperty(document, statusField);
            IndexIsProperty(document, "category");
            IndexIsProperty(document, category.Code);

            document.Add(new IndexDocumentField("status", statusField) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("code", category.Code) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("name", category.Name) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("createddate", category.CreatedDate) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("lastmodifieddate", category.ModifiedDate ?? DateTime.MaxValue) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("modifieddate", category.ModifiedDate ?? DateTime.MaxValue) { IsRetrievable = true, IsFilterable = true });
            document.Add(new IndexDocumentField("priority", category.Priority) { IsRetrievable = true, IsFilterable = true });

            // Add priority in virtual categories to search index
            if (category.Links != null)
            {
                foreach (var link in category.Links)
                {
                    document.Add(new IndexDocumentField($"priority_{link.CatalogId}_{link.CategoryId}", link.Priority) { IsRetrievable = true, IsFilterable = true });
                }
            }

            // Add catalogs to search index
            var catalogs = category.Outlines
                .Select(o => o.Items.First().Id)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            foreach (var catalogId in catalogs)
            {
                document.Add(new IndexDocumentField("catalog", catalogId.ToLowerInvariant()) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
            }

            // Add outlines to search index
            var outlineStrings = GetOutlineStrings(category.Outlines);
            foreach (var outline in outlineStrings)
            {
                document.Add(new IndexDocumentField("__outline", outline.ToLowerInvariant()) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
            }

            IndexCustomProperties(document, category.Properties, category.PropertyValues, new[] { PropertyType.Category });

            // add to content
            document.Add(new IndexDocumentField("__content", category.Name) { IsRetrievable = true, IsSearchable = true, IsCollection = true });
            document.Add(new IndexDocumentField("__content", category.Code) { IsRetrievable = true, IsSearchable = true, IsCollection = true });

            if (StoreObjectsInIndex)
            {
                // Index serialized category
                var itemDto = category.ToWebModel(_blobUrlResolver);
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
