using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Data.Utilities;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    public abstract class OperationEntity : AuditableEntity
    {
        [Required]
        [StringLength(64)]
        public string Number { get; set; }
        public bool IsApproved { get; set; }
        [StringLength(64)]
        public string Status { get; set; }
        [StringLength(2048)]
        public string Comment { get; set; }
        [Required]
        [StringLength(3)]
        public string Currency { get; set; }
        [Column(TypeName = "Money")]
        public decimal Sum { get; set; }

        public bool IsCancelled { get; set; }
        public DateTime? CancelledDate { get; set; }
        [StringLength(2048)]
        public string CancelReason { get; set; }

        public virtual OrderOperation ToModel(OrderOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            operation.Id = Id;
            operation.CreatedDate = CreatedDate;
            operation.CreatedBy = CreatedBy;
            operation.ModifiedDate = ModifiedDate;
            operation.ModifiedBy = ModifiedBy;

            operation.Comment = Comment;
            operation.Currency = Currency;
            operation.Number = Number;
            operation.Status = Status;
            operation.IsCancelled = IsCancelled;
            operation.CancelledDate = CancelledDate;
            operation.CancelReason = CancelReason;
            operation.IsApproved = IsApproved;
            operation.Sum = Sum;
            operation.ChildrenOperations = OperationUtilities.GetAllChildOperations(operation);
            return operation;
        }

        public virtual OperationEntity FromModel(OrderOperation operation, PrimaryKeyResolvingMap pkMap)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            pkMap.AddPair(operation, this);

            Id = operation.Id;
            CreatedDate = operation.CreatedDate;
            CreatedBy = operation.CreatedBy;
            ModifiedDate = operation.ModifiedDate;
            ModifiedBy = operation.ModifiedBy;

            Comment = operation.Comment;
            Currency = operation.Currency;
            Number = operation.Number;
            Status = operation.Status;
            IsCancelled = operation.IsCancelled;
            CancelledDate = operation.CancelledDate;
            CancelReason = operation.CancelReason;
            IsApproved = operation.IsApproved;
            Sum = operation.Sum;

            return this;
        }

        public virtual void Patch(OperationEntity operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            operation.Comment = Comment;
            operation.Currency = Currency;
            operation.Number = Number;
            operation.Status = Status;
            operation.IsCancelled = IsCancelled;
            operation.CancelledDate = CancelledDate;
            operation.CancelReason = CancelReason;
            operation.IsApproved = IsApproved;
            operation.Sum = Sum;
        }
    }
}
