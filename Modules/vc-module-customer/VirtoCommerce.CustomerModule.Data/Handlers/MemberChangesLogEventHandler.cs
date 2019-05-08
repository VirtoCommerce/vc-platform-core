using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Data.ChangeLog;

namespace VirtoCommerce.CustomerModule.Data.Handlers
{
    public class MemberChangesLogEventHandler : ChangesLogEventHandler<Member>
    {
        public MemberChangesLogEventHandler(IChangeLogService changeLogService) : base(changeLogService)
        {
        }
    }
}
