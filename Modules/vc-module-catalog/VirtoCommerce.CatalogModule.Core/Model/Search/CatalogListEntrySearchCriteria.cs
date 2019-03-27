using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    public class CatalogListEntrySearchCriteria : SearchCriteriaBase
    {
        //Hides direct linked categories in virtual category displayed only linked category content without itself
        public bool HideDirectLinkedCategories { get; set; }

        /// <summary>
        /// Search  in all children categories for specified catalog or categories
        /// </summary>
        public bool SearchInChildren { get; set; }
        /// <summary>
        /// Also search in variations 
        /// </summary>
        public bool SearchInVariations { get; set; }


        public string CatalogId { get; set; }

        public string CategoryId { get; set; }
    }
}
