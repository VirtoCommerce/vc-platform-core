using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Data.ChangeLog;

namespace VirtoCommerce.CatalogModule.Data.Handlers
{
    public class CategoryChangesLogEventHandler : ChangesLogEventHandler<Category>
    {
        public CategoryChangesLogEventHandler(IChangeLogService changeLogService) : base(changeLogService)
        {
        }
    }
}
