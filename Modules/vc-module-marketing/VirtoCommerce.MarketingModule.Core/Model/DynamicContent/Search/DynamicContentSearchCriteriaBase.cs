using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.DynamicContent.Search
{
    public class DynamicContentSearchCriteriaBase : SearchCriteriaBase
    {
        public string FolderId { get; set; }
        public string Keyword { get; set; }
    }
}
