using System;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public abstract class ExportDataQuery : ValueObject
    {
        [JsonProperty("exportTypeName")]
        public string ExportTypeName => GetType().Name;
        public string Keyword { get; set; }
        public string[] ObjectIds { get; set; } = new string[] { };
        public string Sort { get; set; }
        public ExportedTypeColumnInfo[] IncludedColumns { get; set; } = Array.Empty<ExportedTypeColumnInfo>();
        public string UserName { get; set; }

        public int? Skip { get; set; }
        public int? Take { get; set; } = 50;

        public abstract SearchCriteriaBase CreateSearchCriteria();

        public virtual SearchCriteriaBase ToSearchCriteria()
        {
            var result = CreateSearchCriteria();

            result.ObjectIds = ObjectIds;
            result.Keyword = Keyword;
            result.Sort = Sort;
            result.Skip = Skip ?? result.Skip;
            result.Take = Take ?? result.Take;

            return result;
        }

        public virtual ExportDataQuery FromSearchCriteria(SearchCriteriaBase searchCriteria)
        {
            ObjectIds = searchCriteria.ObjectIds.ToArray();
            Sort = searchCriteria.Sort;
            Keyword = searchCriteria.Keyword;
            Skip = searchCriteria.Skip;
            Take = searchCriteria.Take;

            return this;
        }
    }
}
