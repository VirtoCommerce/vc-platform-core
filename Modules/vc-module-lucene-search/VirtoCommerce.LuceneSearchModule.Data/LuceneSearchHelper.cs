using System.Collections.Generic;
using System.Globalization;
using Lucene.Net.Index;

namespace VirtoCommerce.LuceneSearchModule.Data
{
    public static class LuceneSearchHelper
    {
        public const string KeyFieldName = "__id";
        public const string SearchableFieldName = "__all";
        public const string BooleanFieldSuffix = ".boolean";
        public const string DateTimeFieldSuffix = ".datetime";
        public const string FacetableFieldSuffix = ".facetable";

        public static readonly string[] SearchableFields = { SearchableFieldName };

        public static string ToLuceneFieldName(string originalName)
        {
            return originalName?.ToLowerInvariant();
        }

        public static string GetBooleanFieldName(string originalName)
        {
            return ToLuceneFieldName(originalName + BooleanFieldSuffix);
        }

        public static string GetDateTimeFieldName(string originalName)
        {
            return ToLuceneFieldName(originalName + DateTimeFieldSuffix);
        }

        public static string GetFacetableFieldName(string originalName)
        {
            return ToLuceneFieldName(originalName + FacetableFieldSuffix);
        }

        public static string ToStringInvariant(this object value)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", value);
        }

        //TODO
        //public static IEnumerable<string> GetAllFieldValues(this IndexReader reader, string field)
        //{
        //    if (string.IsNullOrEmpty(field))
        //    {
        //        yield break;
        //    }

        //    var termEnum = reader.Terms(new Term(field));

        //    do
        //    {
        //        var currentTerm = termEnum.Term;

        //        if (currentTerm == null || currentTerm.Field != field)
        //            yield break;

        //        yield return currentTerm.Text;
        //    }
        //    while (termEnum.Next());
        //}
    }
}
