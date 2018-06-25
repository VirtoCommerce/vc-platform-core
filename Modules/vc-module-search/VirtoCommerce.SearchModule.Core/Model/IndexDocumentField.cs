using System.Collections.Generic;
using System.Diagnostics;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SearchModule.Core.Model
{
    [DebuggerDisplay("{Name}: {string.Join(\", \", Values)}")]
    public class IndexDocumentField
    {
        public IndexDocumentField(string name, object value)
        {
            Name = name;
            Values = new List<object> { value };
        }

        public IndexDocumentField(string name, IList<object> values)
        {
            Name = name;
            Values = values;
        }

        public string Name { get; set; }
        public IList<object> Values { get; set; }

        public object Value
        {
            get
            {
                if (Values != null && Values.Count > 0)
                    return Values[0];

                return null;
            }
        }

        // Meta information required for indexing:

        public bool IsRetrievable { get; set; }
        public bool IsFilterable { get; set; }
        public bool IsSearchable { get; set; }
        public bool IsCollection { get; set; }

        public void Merge(IndexDocumentField field)
        {
            foreach (var value in field.Values)
            {
                Values.AddDistinct(value);
            }
        }
    }
}
