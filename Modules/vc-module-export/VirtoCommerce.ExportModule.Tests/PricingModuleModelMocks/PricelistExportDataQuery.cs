using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Tests.PricingModuleModelMocks
{
    public class PricelistExportDataQuery : ExportDataQuery
    {
        public override SearchCriteriaBase CreateSearchCriteria()
        {
            return new PricelistSearchCriteria();
        }
    }

    public class PricelistSearchCriteria : SearchCriteriaBase
    {
    }
}
