using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public abstract class ExportDataQuery : ValueObject
    {
        public string ExportTypeName { get; set; }
        public string[] ObjectIds { get; set; } = new string[] { };
        public string Sort { get; set; }
        public string[] IncludedProperties { get; set; } = new string[] { };

        public abstract SearchCriteriaBase CreateSearchCriteria();

        public virtual SearchCriteriaBase ToSearchCriteria()
        {
            var result = CreateSearchCriteria();
            result.ObjectIds = ObjectIds;
            result.Sort = Sort;
            return result;
        }

        public virtual ExportDataQuery FromSearchCriteria(SearchCriteriaBase searchCriteria)
        {
            ObjectIds = searchCriteria.ObjectIds.ToArray();
            Sort = searchCriteria.Sort;
            return this;
        }
    }
}
