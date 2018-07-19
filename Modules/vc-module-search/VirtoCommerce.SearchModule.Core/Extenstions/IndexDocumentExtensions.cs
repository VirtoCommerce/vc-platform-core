using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Core.Extenstions
{
    public static class IndexDocumentExtensions
    {
        public const string SearchableFieldName = "__content";

        /// <summary>
        ///  Adds given values to the filterable field with given name and to the searchable '__content' field
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <param name="values"></param>
        public static void AddFilterableAndSearchableValues(this IndexDocument document, string name, ICollection<string> values)
        {
            if (values?.Any() == true)
            {
                foreach (var value in values)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        document.Add(new IndexDocumentField(name, value) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
                        document.AddSearchableValue(value);
                    }
                }
            }
        }

        /// <summary>
        ///  Adds given value to the filterable field with given name and to the searchable '__content' field
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void AddFilterableAndSearchableValue(this IndexDocument document, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                document.Add(new IndexDocumentField(name, value) { IsRetrievable = true, IsFilterable = true });
                document.AddSearchableValue(value);
            }
        }

        /// <summary>
        ///  Adds given value to the searchable '__content' field
        /// </summary>
        /// <param name="document"></param>
        /// <param name="value"></param>
        public static void AddSearchableValue(this IndexDocument document, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                document.Add(new IndexDocumentField(SearchableFieldName, value) { IsRetrievable = true, IsSearchable = true, IsCollection = true });
            }
        }

        public static void AddFilterableValue(this IndexDocument document, string name, object value)
        {
            if (value != null)
            {
                document.Add(new IndexDocumentField(name, value) { IsRetrievable = true, IsFilterable = true });
            }
        }

        public static void AddFilterableValues(this IndexDocument document, string name, ICollection<string> values)
        {
            if (values?.Any() == true)
            {
                foreach (var value in values)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        document.Add(new IndexDocumentField(name, value) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
                    }
                }
            }
        }
    }
}
