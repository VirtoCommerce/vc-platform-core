namespace VirtoCommerce.CatalogModule.Core2.Model.Search.Indexed
{
    public class AggregationItem
    {
        /// <summary>
        /// Gets or sets the aggregation item value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the aggregation item count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the flag for aggregation item is applied
        /// </summary>
        public bool IsApplied { get; set; }

        /// <summary>
        /// Gets or sets the collection of the aggregation item labels
        /// </summary>
        public AggregationLabel[] Labels { get; set; }

        /// <summary>
        /// Gets or sets the request lower bound for range aggregation value
        /// </summary>
        public string RequestedLowerBound { get; set; }

        /// <summary>
        /// Gets or sets the request lower bound for range aggregation value
        /// </summary>
        public string RequestedUpperBound { get; set; }
    }
}
