using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.SubscriptionModule.Core.Events;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Data.Resources;

namespace VirtoCommerce.SubscriptionModule.Data.Handlers
{
    public class LogChangesSubscriptionChangedEventHandler : IEventHandler<SubscriptionChangedEvent>, IEventHandler<OrderChangedEvent>
    {
        private readonly IChangeLogService _changeLogService;

        public LogChangesSubscriptionChangedEventHandler(IChangeLogService changeLogService)
        {
            _changeLogService = changeLogService;
        }

        public virtual async Task Handle(OrderChangedEvent message)
        {
            var operationLogs = new List<OperationLog>();
            foreach (var changedEntry in message.ChangedEntries)
            {
                operationLogs.AddRange(await GetChangedEntryOperationLogsAsync(changedEntry));
            }
            if (!operationLogs.IsNullOrEmpty())
            {
                await _changeLogService.SaveChangesAsync(operationLogs.ToArray());
            }
        }

        public virtual async Task Handle(SubscriptionChangedEvent message)
        {
            var operationLogs = new List<OperationLog>();
            foreach (var changedEntry in message.ChangedEntries)
            {
                operationLogs.AddRange(await GetChangedEntryOperationLogsAsync(changedEntry));
            }
            if (!operationLogs.IsNullOrEmpty())
            {
                await _changeLogService.SaveChangesAsync(operationLogs.ToArray());
            }
        }

        protected virtual Task<IEnumerable<OperationLog>> GetChangedEntryOperationLogsAsync(GenericChangedEntry<CustomerOrder> changedEntry)
        {
            var result = new List<OperationLog>();

            //log for new recurring orders creation
            //handle only recurring orders
            if (changedEntry.EntryState == EntryState.Added && !string.IsNullOrEmpty(changedEntry.OldEntry.SubscriptionId))
            {
                var operationLog = GetLogRecord(changedEntry.NewEntry.SubscriptionId, SubscriptionResources.NewRecurringOrderCreated, changedEntry.NewEntry.Number);
                result.Add(operationLog);
            }
            return Task.FromResult<IEnumerable<OperationLog>>(result);
        }

        protected virtual Task<IEnumerable<OperationLog>> GetChangedEntryOperationLogsAsync(GenericChangedEntry<Subscription> changedEntry)
        {
            var result = new List<OperationLog>();

            var original = changedEntry.OldEntry;
            var modified = changedEntry.NewEntry;

            if (changedEntry.EntryState == EntryState.Modified)
            {
                if (original.SubscriptionStatus != modified.SubscriptionStatus)
                {
                    result.Add(GetLogRecord(modified.Id, SubscriptionResources.StatusChanged, original.SubscriptionStatus, modified.SubscriptionStatus));
                }
                if (original.Interval != modified.Interval)
                {
                    result.Add(GetLogRecord(modified.Id, SubscriptionResources.IntervalChanged, original.Interval, modified.Interval));
                }
                if (original.IntervalCount != modified.IntervalCount)
                {
                    result.Add(GetLogRecord(modified.Id, SubscriptionResources.IntervalCountChanged, original.IntervalCount, modified.IntervalCount));
                }
                if (original.TrialPeriodDays != modified.TrialPeriodDays)
                {
                    result.Add(GetLogRecord(modified.Id, SubscriptionResources.TrialPeriodChanged, original.TrialPeriodDays, modified.TrialPeriodDays));
                }
                if (original.CurrentPeriodEnd != modified.CurrentPeriodEnd)
                {
                    result.Add(GetLogRecord(modified.Id, SubscriptionResources.NextBillingDateChanged, original.CurrentPeriodEnd, modified.CurrentPeriodEnd));
                }
                if (original.Balance != modified.Balance)
                {
                    result.Add(GetLogRecord(modified.Id, SubscriptionResources.BalanceChanged, original.Balance, modified.Balance));
                }
                if (modified.IsCancelled && original.IsCancelled != modified.IsCancelled)
                {
                    result.Add(GetLogRecord(modified.Id, SubscriptionResources.SubscriptionCanceled, modified.CancelledDate, modified.CancelReason ?? ""));
                }
                if (original.OuterId != modified.OuterId)
                {
                    result.Add(GetLogRecord(modified.Id, SubscriptionResources.OuterIdChanged, original.OuterId, modified.OuterId));
                }
            }

            return Task.FromResult<IEnumerable<OperationLog>>(result);
        }

        protected virtual OperationLog GetLogRecord(string subscriptionId, string template, params object[] parameters)
        {
            var result = new OperationLog
            {
                ObjectId = subscriptionId,
                ObjectType = typeof(Subscription).Name,
                OperationType = EntryState.Modified,
                Detail = string.Format(template, parameters)
            };
            return result;
        }


    }
}
