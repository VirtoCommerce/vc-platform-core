using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CatalogModule.Data.Search.Indexing
{
    public abstract class CatalogDocumentBuilder
    {
        private readonly ISettingsManager _settingsManager;

        protected CatalogDocumentBuilder(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        protected virtual bool StoreObjectsInIndex => _settingsManager.GetValue("Catalog.Search.UseFullObjectIndexStoring", true);

        [Obsolete("Use overload with contentPropertyTypes")]
        protected virtual void IndexCustomProperties(IndexDocument document, ICollection<Property> properties, ICollection<PropertyValue> propertyValues)
        {
            IndexCustomProperties(document, properties, propertyValues, null);
        }

        protected virtual void IndexCustomProperties(IndexDocument document, ICollection<Property> properties, ICollection<PropertyValue> propertyValues, ICollection<PropertyType> contentPropertyTypes)
        {
            foreach (var propValue in propertyValues.Where(x => x.Value != null))
            {
                var property = properties.FirstOrDefault(p => p.Name.EqualsInvariant(propValue.PropertyName) && p.ValueType == propValue.ValueType);

                var propertyName = propValue.PropertyName?.ToLowerInvariant();
                if (!string.IsNullOrEmpty(propertyName))
                {
                    var isCollection = property?.Multivalue == true;

                    switch (propValue.ValueType)
                    {
                        case PropertyValueType.Boolean:
                        case PropertyValueType.DateTime:
                        case PropertyValueType.Integer:
                        case PropertyValueType.Number:
                            document.Add(new IndexDocumentField(propertyName, propValue.Value) { IsRetrievable = true, IsFilterable = true, IsCollection = isCollection });
                            break;
                        case PropertyValueType.LongText:
                            document.Add(new IndexDocumentField(propertyName, propValue.Value.ToString().ToLowerInvariant()) { IsRetrievable = true, IsSearchable = true, IsCollection = isCollection });
                            break;
                        case PropertyValueType.ShortText:
                            // Index alias when it is available instead of display value.
                            // Do not tokenize small values as they will be used for lookups and filters.
                            var shortTextValue = propValue.Alias ?? propValue.Value.ToString();
                            document.Add(new IndexDocumentField(propertyName, shortTextValue) { IsRetrievable = true, IsFilterable = true, IsCollection = isCollection });
                            break;
                        case PropertyValueType.GeoPoint:
                            document.Add(new IndexDocumentField(propertyName, GeoPoint.TryParse((string)propValue.Value)) { IsRetrievable = true, IsSearchable = true, IsCollection = true });
                            break;
                    }
                }

                // Add value to the searchable content field if property type is uknown or if it is present in the provided list
                if (property == null || contentPropertyTypes == null || contentPropertyTypes.Contains(property.Type))
                {
                    var contentField = string.Concat("__content", property != null && property.Multilanguage && !string.IsNullOrWhiteSpace(propValue.LanguageCode) ? "_" + propValue.LanguageCode.ToLowerInvariant() : string.Empty);

                    switch (propValue.ValueType)
                    {
                        case PropertyValueType.LongText:
                        case PropertyValueType.ShortText:
                            var stringValue = propValue.Value.ToString();

                            if (!string.IsNullOrWhiteSpace(stringValue)) // don't index empty values
                            {
                                document.Add(new IndexDocumentField(contentField, stringValue.ToLower()) { IsRetrievable = true, IsSearchable = true, IsCollection = true });
                            }

                            break;
                    }
                }
            }
        }

        protected virtual string[] GetOutlineStrings(IEnumerable<Outline> outlines)
        {
            return outlines
                .SelectMany(ExpandOutline)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        protected virtual IEnumerable<string> ExpandOutline(Outline outline)
        {
            // Outline structure: catalog/category1/.../categoryN/current-item

            var items = outline.Items
                .Take(outline.Items.Count - 1) // Exclude last item, which is current item ID
                .Select(i => i.Id)
                .ToList();

            var result = new List<string>();

            // Add partial outline for each parent:
            // catalog/category1/category2
            // catalog/category1
            // catalog
            if (items.Count > 0)
            {
                for (var i = items.Count; i > 0; i--)
                {
                    result.Add(string.Join("/", items.Take(i)));
                }
            }

            // For each parent category create a separate outline: catalog/parent_category
            if (items.Count > 2)
            {
                var catalogId = items.First();

                result.AddRange(
                    items.Skip(1)
                        .Select(i => string.Join("/", catalogId, i)));
            }

            return result;
        }
    }
}
