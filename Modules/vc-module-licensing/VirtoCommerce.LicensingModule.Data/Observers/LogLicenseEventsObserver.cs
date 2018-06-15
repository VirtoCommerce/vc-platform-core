using System;
using System.Collections.Generic;
using VirtoCommerce.LicensingModule.Core.Events;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Data.Observers
{
    public class LogLicenseEventsObserver : IObserver<LicenseSignedEvent>, IObserver<LicenseChangedEvent>
    {
        private readonly IChangeLogService _changeLogService;

        public LogLicenseEventsObserver(IChangeLogService changeLogService)
        {
            _changeLogService = changeLogService;
        }

        #region IObserver<LicenseActivateEvent>
        public void OnNext(LicenseSignedEvent value)
        {
            var template = value.IsActivated ? "License activated from IP '{0}'" : "License downloaded from IP '{0}'";
            var log = GetLogRecord(value.License.Id, template, value.ClientIpAddress);
            _changeLogService.SaveChanges(log);
        }
        #endregion

        #region IObserver<LicenseChangeEvent>
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(LicenseChangedEvent value)
        {
            foreach (var changedEntry in value.ChangedEntries)
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

                    _changeLogService.SaveChanges(operationLogs.ToArray());
                }
            }
            
        }
        #endregion

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
