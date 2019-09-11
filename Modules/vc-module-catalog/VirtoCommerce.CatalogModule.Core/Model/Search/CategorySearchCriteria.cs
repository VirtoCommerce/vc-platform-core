using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    public class CategorySearchCriteria : SearchCriteriaBase
    {
        public string CatalogId { get; set; }

        private string[] _catalogIds;
        public string[] CatalogIds
        {
            get
            {
                if (_catalogIds == null && !string.IsNullOrEmpty(CatalogId))
                {
                    _catalogIds = new[] { CatalogId };
                }
                return _catalogIds;
            }
            set
            {
                _catalogIds = value;
            }
        }


        /// <summary>
        /// Parent category id
        /// </summary>
        public string CategoryId { get; set; }
    }
}
