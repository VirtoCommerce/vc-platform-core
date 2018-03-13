using System.Diagnostics;

namespace VirtoCommerce.Domain.Search
{
    [DebuggerDisplay("DISTANCE {FieldName} {Location} {IsDescending ? \"desc\" : \"asc\"}")]
    public class GeoDistanceSortingField : SortingField
    {
        public GeoPoint Location { get; set; }
    }
}
