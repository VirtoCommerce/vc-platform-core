using System.Collections.Generic;

namespace VirtoCommerce.StoreModule.Core.Model.Search
{
    public class StoreSearchResult
    {
        public StoreSearchResult()
        {
            Stores = new List<Store>();
        }

        public int TotalCount { get; set; }
        /// <summary>
        /// Type used in search result and represent properties search result aggregation 
        /// </summary>
        public ICollection<Store> Stores { get; set; }
 

    }
}
