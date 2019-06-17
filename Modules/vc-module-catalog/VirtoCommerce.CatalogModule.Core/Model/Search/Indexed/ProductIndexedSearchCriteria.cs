using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    public class ProductIndexedSearchCriteria : CatalogIndexedSearchCriteria
    {
        public override string ObjectType { get; set; } = KnownDocumentTypes.Product;

        /// <summary>
        /// Physical, Digital, etc.
        /// </summary>
        public string ProductType { get; set; }

        public string Currency { get; set; }

        public string[] Pricelists { get; set; }

        public NumericRange PriceRange { get; set; }

        /// <summary>
        /// Gets or sets the class types.
        /// </summary>
        /// <value>The class types.</value>
        public virtual IList<string> ClassTypes { get; set; } = new List<string>();

        /// <summary>
        /// Specifies if we search for hidden products.
        /// </summary>
        public virtual bool WithHidden { get; set; }

        /// <summary>
        /// Gets or sets the start date. The date must be in UTC format as that is format indexes are stored in.
        /// </summary>
        /// <value>The start date.</value>
        public virtual DateTime StartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the start date from filter. Used for filtering new products. The date must be in UTC format as that is format indexes are stored in.
        /// </summary>
        /// <value>The start date from.</value>
        public virtual DateTime? StartDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the end date. The date must be in UTC format as that is format indexes are stored in.
        /// </summary>
        /// <value>The end date.</value>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets a "white" list of aggregation keys that identify preconfigured aggregations, which SHOULD be calculated and returned with the search result.
        /// </summary>
        public IList<string> IncludeAggregations { get; set; }

        /// <summary>
        /// Gets or sets a "black" list of aggregation keys that identify preconfigured aggregations, which SHOULD NOT be calculated and returned with the search result.
        /// </summary>
        public IList<string> ExcludeAggregations { get; set; }

        /// <summary>
        /// Geo distance filter
        /// </summary>
        public GeoDistanceFilter GeoDistanceFilter { get; set; }

        /// <summary>
        /// Override base SortInfo property to support GeoSortInfo sorting types
        /// </summary>
        public override IList<SortInfo> SortInfos => GeoSortInfo.TryParse(Sort).ToList();
    }
}
