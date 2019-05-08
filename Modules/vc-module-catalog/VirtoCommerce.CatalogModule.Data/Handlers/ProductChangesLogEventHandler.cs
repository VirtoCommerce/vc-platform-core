using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Data.ChangeLog;

namespace VirtoCommerce.CatalogModule.Data.Handlers
{
    public class ProductChangesLogEventHandler : ChangesLogEventHandler<CatalogProduct>
    {
        public ProductChangesLogEventHandler(IChangeLogService changeLogService) : base(changeLogService)
        {
        }
    }
}
