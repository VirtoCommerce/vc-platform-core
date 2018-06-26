using System.Diagnostics;

namespace VirtoCommerce.SearchModule.Core.Model
{
    [DebuggerDisplay("{FieldName} {IsDescending ? \"desc\" : \"asc\"}")]
    public class SortingField
    {
        public string FieldName { get; set; }
        public bool IsDescending { get; set; }

        public SortingField()
        {
        }

        public SortingField(string fieldName)
        {
            FieldName = fieldName;
        }

        public SortingField(string fieldName, bool isDescending)
        {
            FieldName = fieldName;
            IsDescending = isDescending;
        }
    }
}
