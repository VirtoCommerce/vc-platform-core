using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core2.Model.Search
{
    public class CatalogListEntrySearchCriteria : SearchCriteriaBase
    {
        //Hides direct linked categories in virtual category displayed only linked category content without itself
        public bool HideDirectLinkedCategories { get; set; }

        public string CatalogId { get; set; }

        public string CategoryId { get; set; }
    }
}
