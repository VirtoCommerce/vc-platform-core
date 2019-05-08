using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Data.ChangeLog;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.Handlers
{
    public class PriceChangesLogEventHandler : ChangesLogEventHandler<Price>
    {
        public PriceChangesLogEventHandler(IChangeLogService changeLogService) : base(changeLogService)
        {
        }
    }
}
