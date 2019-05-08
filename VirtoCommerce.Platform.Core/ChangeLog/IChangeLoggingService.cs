using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.ChangeLog
{
    public interface IChangeLogService
    {
        Task SaveChangesAsync(params OperationLog[] operationLogs);
        IEnumerable<OperationLog> FindChangeHistory(string objectType, DateTime? startDate, DateTime? endDate);
        IEnumerable<OperationLog> FindObjectChangeHistory(string objectId, string objectType);
        OperationLog GetObjectLastChange(string objectId, string objectType);
        void LoadChangeLogs(IHasChangesHistory owner);
    }
}
