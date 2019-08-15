using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.MarketingModule.Core.Events;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.MarketingModule.Data.Handlers
{
    public class LogChangesChangedEventHandler : IEventHandler<PromotionChangedEvent>, IEventHandler<CouponChangedEvent>
    {
        private readonly IChangeLogService _changeLogService;

        public LogChangesChangedEventHandler(IChangeLogService changeLogService)
        {
            _changeLogService = changeLogService;
        }

        public virtual Task Handle(PromotionChangedEvent @event)
        {
            InnerHandle(@event);
            return Task.CompletedTask;
        }

        public virtual Task Handle(CouponChangedEvent @event)
        {
            InnerHandle(@event);
            return Task.CompletedTask;
        }

        protected virtual void InnerHandle<T>(GenericChangedEntryEvent<T> @event) where T : IEntity
        {
            var logOperations = @event.ChangedEntries.Select(x => AbstractTypeFactory<OperationLog>.TryCreateInstance().FromChangedEntry(x)).ToArray();
            //Background task is used here for performance reasons
            BackgroundJob.Enqueue(() => LogEntityChangesInBackground(logOperations));
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public void LogEntityChangesInBackground(OperationLog[] operationLogs)
        {
            _changeLogService.SaveChangesAsync(operationLogs).GetAwaiter().GetResult();
        }
    }
}
