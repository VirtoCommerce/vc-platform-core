using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public abstract class CatalogIndexedSearchResult<TItem>
    {
        public virtual long TotalCount { get; set; }

        public virtual TItem[] Items { get; set; }

        public virtual Aggregation[] Aggregations { get; set; }
    }
}
