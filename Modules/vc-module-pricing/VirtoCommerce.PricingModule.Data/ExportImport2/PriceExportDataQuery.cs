using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.Search;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{

    public class PriceExportDataQuery : ExportDataQuery
    {
        public override SearchCriteriaBase ToSearchCriteria()
        {
            var result = new PricesSearchCriteria();
            result.ObjectIds = ObjectIds;
            result.Sort = Sort;
            return result;
        }

        public override void FromSearchCriteria(SearchCriteriaBase searchCriteria)
        {
            ObjectIds = searchCriteria.ObjectIds.ToArray();
            Sort = searchCriteria.Sort;
        }
    }
}
