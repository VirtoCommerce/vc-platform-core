using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Model;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.Platform.Data.ChangeLog
{
    public class ChangeLogService : ServiceBase, IChangeLogService
    {
        private readonly Func<IPlatformRepository> _platformRepositoryFactory;

        public ChangeLogService(Func<IPlatformRepository> platformRepositoryFactory)
        {
            _platformRepositoryFactory = platformRepositoryFactory;
        }

        #region IChangeLogService Members

        public void LoadChangeLogs(IHasChangesHistory owner)
        {
            var objectsWithChangesHistory = owner.GetFlatObjectsListWithInterface<IHasChangesHistory>().Distinct();

            foreach (var objectWithChangesHistory in objectsWithChangesHistory)
            {
                if (objectWithChangesHistory.Id != null)
                {
                    objectWithChangesHistory.OperationsLog = FindObjectChangeHistory(objectWithChangesHistory.Id, objectWithChangesHistory.GetType().Name).ToList();
                }
            }
        }

        public void SaveChanges(params OperationLog[] operationLogs)
        {
            if (operationLogs == null)
            {
                throw new ArgumentNullException(nameof(operationLogs));
            }
            var pkMap = new PrimaryKeyResolvingMap();

            using (var repository = _platformRepositoryFactory())
            {
                var ids = operationLogs.Where(x => x.Id != null).Select(x => x.Id).Distinct().ToArray();
                var origDbOperations = repository.OperationLogs.Where(x => ids.Contains(x.Id));
                foreach (var operation in operationLogs)
                {
                    var originalEntity = origDbOperations.FirstOrDefault(x => x.Id == operation.Id);
                    var modifiedEntity = AbstractTypeFactory<OperationLogEntity>.TryCreateInstance().FromModel(operation, pkMap);
                    if (originalEntity != null)
                    {
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                    }
                }
                repository.UnitOfWork.Commit();
            }
        }

        public IEnumerable<OperationLog> FindObjectChangeHistory(string objectId, string objectType)
        {
            if (objectId == null)
                throw new ArgumentNullException(nameof(objectId));

            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            using (var repository = _platformRepositoryFactory())
            {
                var retVal = repository.OperationLogs.Where(x => x.ObjectId == objectId && x.ObjectType == objectType)
                                                    .OrderBy(x => x.ModifiedDate).ToArray()
                                                    .Select(x => x.ToModel(AbstractTypeFactory<OperationLog>.TryCreateInstance()))
                                                    .ToList();
                return retVal;
            }
        }

        public OperationLog GetObjectLastChange(string objectId, string objectType)
        {
            if (objectId == null)
                throw new ArgumentNullException(nameof(objectId));

            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            OperationLog retVal = null;
            using (var repository = _platformRepositoryFactory())
            {
                var entity = repository.OperationLogs.Where(x => x.ObjectId == objectId && x.ObjectType == objectType)
                                                     .OrderByDescending(x => x.ModifiedDate).FirstOrDefault();
                if (entity != null)
                {
                    retVal = entity.ToModel(AbstractTypeFactory<OperationLog>.TryCreateInstance());
                }

            }
            return retVal;
        }

        public IEnumerable<OperationLog> FindChangeHistory(string objectType, DateTime? startDate, DateTime? endDate)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            using (var repository = _platformRepositoryFactory())
            {
                var retVal = repository.OperationLogs.Where(x => x.ObjectType == objectType && (startDate == null || x.ModifiedDate >= startDate) && (endDate == null || x.ModifiedDate <= endDate))
                                                 .OrderBy(x => x.ModifiedDate).ToArray()
                                                 .Select(x => x.ToModel(AbstractTypeFactory<OperationLog>.TryCreateInstance()))
                                                 .ToList();
                return retVal;
            }
        }

        #endregion
    }
}
