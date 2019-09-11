using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.OrdersModule.Core.Model
{
    public abstract class OrderOperation : AuditableEntity, IOperation, ISupportCancellation, IHasDynamicProperties, IHasChangesHistory, ICloneable
    {
        public OrderOperation()
        {
            OperationType = GetType().Name;
        }
        public string OperationType { get; set; }
        public string ParentOperationId { get; set; }
        public string Number { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; }

        public string Comment { get; set; }
        public string Currency { get; set; }
        public decimal Sum { get; set; }
        public string OuterId { get; set; }

        public IEnumerable<IOperation> ChildrenOperations { get; set; }

        #region ISupportCancelation Members

        public bool IsCancelled { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string CancelReason { get; set; }

        #endregion

        #region IHasDynamicProperties Members

        public string ObjectType => typeof(CustomerOrder).FullName;
        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }

        #endregion

        #region IHasChangesHistory
        public ICollection<OperationLog> OperationsLog { get; set; }
        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as OrderOperation;
            
            result.DynamicProperties = DynamicProperties?.Select(x => x.Clone()).OfType<DynamicObjectProperty>().ToList();
            result.OperationsLog = OperationsLog?.Select(x => x.Clone()).OfType<OperationLog>().ToList();

            return result;
        }

        #endregion
    }
}
