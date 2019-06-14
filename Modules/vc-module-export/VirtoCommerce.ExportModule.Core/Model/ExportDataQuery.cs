using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public abstract class ExportDataQuery : ValueObject
    {
        public string ExportTypeName { get; set; }
        public string[] ObjectIds { get; set; }
        public string Sort { get; set; }
        public string[] IncludedProperties { get; set; }

        public abstract SearchCriteriaBase ToSearchCriteria();
        public abstract void FromSearchCriteria(SearchCriteriaBase searchCriteria);
    }
}
