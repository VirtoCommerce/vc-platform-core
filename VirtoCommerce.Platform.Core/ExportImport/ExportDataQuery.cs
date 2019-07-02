using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.ExportImport
{
    public abstract class ExportDataQuery : ValueObject
    {
        [JsonProperty("exportTypeName")]
        public string ExportTypeName => GetType().Name;
        public string Keyword { get; set; }
        public string[] ObjectIds { get; set; } = new string[] { };
        public string Sort { get; set; }
        public string[] IncludedProperties { get; set; } = new string[] { };

        public abstract SearchCriteriaBase CreateSearchCriteria();

        public virtual SearchCriteriaBase ToSearchCriteria()
        {
            var result = CreateSearchCriteria();
            result.ObjectIds = ObjectIds;
            result.Keyword = Keyword;
            result.Sort = Sort;
            return result;
        }

        public virtual ExportDataQuery FromSearchCriteria(SearchCriteriaBase searchCriteria)
        {
            ObjectIds = searchCriteria.ObjectIds.ToArray();
            Sort = searchCriteria.Sort;
            Keyword = searchCriteria.Keyword;
            return this;
        }
    }
}
