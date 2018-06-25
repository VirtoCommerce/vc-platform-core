using System.Diagnostics;

namespace VirtoCommerce.SearchModule.Core.Model
{
    [DebuggerDisplay("DISTANCE {FieldName} {Location} {IsDescending ? \"desc\" : \"asc\"}")]
    public class GeoDistanceSortingField : SortingField
    {
        public GeoPoint Location { get; set; }
    }
}
