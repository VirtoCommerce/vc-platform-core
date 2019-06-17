using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.LicensingModule.Core.Events;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.LicensingModule.Data.Handlers
{
    public class LogLicenseChangedEventHandler : IEventHandler<LicenseSignedEvent>, IEventHandler<LicenseChangedEvent>
    {
        private readonly IChangeLogService _changeLogService;

        public LogLicenseChangedEventHandler(IChangeLogService changeLogService)
        {
            _changeLogService = changeLogService;
        }

        public virtual async Task Handle(LicenseSignedEvent message)
        {
            await SaveOperationLogAsync(message.License.Id, message.IsActivated ? $"License activated from IP '{message.ClientIpAddress}'" : $"License downloaded from IP '{message.ClientIpAddress}'", EntryState.Modified);
        }

        public async Task Handle(LicenseChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                var original = changedEntry.OldEntry;
                var modified = changedEntry.NewEntry;

                if (changedEntry.EntryState == EntryState.Modified)
                {
                    var operationLogs = new List<OperationLog>();

                    if (original.CustomerName != modified.CustomerName)
                    {
                        operationLogs.Add(GetLogRecord(modified.Id, "Customer name changed from '{0}' to '{1}'", original.CustomerName, modified.CustomerName));
                    }
                    if (original.CustomerEmail != modified.CustomerEmail)
                    {
                        operationLogs.Add(GetLogRecord(modified.Id, "Customer email changed from '{0}' to '{1}'", original.CustomerEmail, modified.CustomerEmail));
                    }
                    if (original.ExpirationDate != modified.ExpirationDate)
                    {
                        operationLogs.Add(GetLogRecord(modified.Id, "Expiration date changed from '{0}' to '{1}'", original.ExpirationDate, modified.ExpirationDate));
                    }
                    if (original.Type != modified.Type)
                    {
                        operationLogs.Add(GetLogRecord(modified.Id, "License type changed from '{0}' to '{1}'", original.Type, modified.Type));
                    }
                    if (original.ActivationCode != modified.ActivationCode)
                    {
                        operationLogs.Add(GetLogRecord(modified.Id, "Activation code changed from '{0}' to '{1}'", original.ActivationCode, modified.ActivationCode));
                    }

                    await _changeLogService.SaveChangesAsync(operationLogs.ToArray());
                }
            }
        }

        protected virtual async Task SaveOperationLogAsync(string objectId, string detail, EntryState entryState)
        {
            var operation = new OperationLog
            {
                ObjectId = objectId,
                ObjectType = typeof(License).Name,
                OperationType = entryState,
                Detail = detail
            };
            await _changeLogService.SaveChangesAsync(operation);
        }

        private static OperationLog GetLogRecord(string licenseId, string template, params object[] parameters)
        {
            return new OperationLog
            {
                ObjectId = licenseId,
                ObjectType = typeof(License).Name,
                OperationType = EntryState.Modified,
                Detail = string.Format(template, parameters)
            };
        }
    }
}
