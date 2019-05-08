using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.Platform.Data.ChangeLog
{
    public class ChangesLogEventHandler<T> : IEventHandler<ChangesLogEvent<T>> where T : IChangesLog
    {
        private readonly IChangeLogService _changeLogService;

        public ChangesLogEventHandler(IChangeLogService changeLogService)
        {
            _changeLogService = changeLogService;
        }

        public async Task Handle(ChangesLogEvent<T> message)
        {
            var operations = message.ChangedEntries.Where(x => x.EntryState != EntryState.Unchanged)
                .Select(e => new OperationLog
                {
                    ObjectId = e.NewEntry.Id,
                    ObjectType = e.NewEntry.GetType().Name,
                    OperationType = e.EntryState,
                }).ToArray();
            await _changeLogService.SaveChangesAsync(operations);
        }
    }
}
