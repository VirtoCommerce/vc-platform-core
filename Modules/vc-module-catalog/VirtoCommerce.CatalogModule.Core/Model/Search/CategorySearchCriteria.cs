using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    public class CategorySearchCriteria : SearchCriteriaBase
    {
        public string CatalogId { get; set; }
        /// <summary>
        /// Parent category id
        /// </summary>
        public string CategoryId { get; set; }
    }
}
