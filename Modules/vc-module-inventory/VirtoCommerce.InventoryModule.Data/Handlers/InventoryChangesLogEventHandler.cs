using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Data.ChangeLog;

namespace VirtoCommerce.InventoryModule.Data.Handlers
{
    public class InventoryChangesLogEventHandler : ChangesLogEventHandler<InventoryInfo>
    {
        public InventoryChangesLogEventHandler(IChangeLogService changeLogService) : base(changeLogService)
        {
        }
    }
}
