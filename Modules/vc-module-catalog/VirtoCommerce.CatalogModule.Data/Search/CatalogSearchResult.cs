using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public abstract class CatalogSearchResult<TItem>
    {
        public virtual long TotalCount { get; set; }

        public virtual TItem[] Items { get; set; }

        public virtual Aggregation[] Aggregations { get; set; }
    }
}
