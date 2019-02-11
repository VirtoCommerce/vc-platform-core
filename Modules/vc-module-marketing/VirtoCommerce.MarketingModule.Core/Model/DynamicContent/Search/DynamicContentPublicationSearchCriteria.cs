namespace VirtoCommerce.MarketingModule.Core.Model
{
    public class DynamicContentPublicationSearchCriteria : DynamicContentSearchCriteriaBase
    {
        public bool OnlyActive { get; set; }
        public string Store { get; set; }
    }
}
