namespace VirtoCommerce.Domain.Search
{
    public class AggregationRequest
    {
        public string Id { get; set; }
        public string FieldName { get; set; }
        public IFilter Filter { get; set; }
    }
}
