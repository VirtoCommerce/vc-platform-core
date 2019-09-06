using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    public abstract class OperationEntity : AuditableEntity, IHasOuterId
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

        [NotMapped]
        public bool NeedPatchSum { get; set; } = true;

        [StringLength(128)]
        public string OuterId { get; set; }

        public virtual OrderOperation ToModel(OrderOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            operation.Id = Id;
            operation.CreatedDate = CreatedDate;
            operation.CreatedBy = CreatedBy;
            operation.ModifiedDate = ModifiedDate;
            operation.ModifiedBy = ModifiedBy;
            operation.OuterId = OuterId;

            operation.Comment = Comment;
            operation.Currency = Currency;
            operation.Number = Number;
            operation.Status = Status;
            operation.IsCancelled = IsCancelled;
            operation.CancelledDate = CancelledDate;
            operation.CancelReason = CancelReason;
            operation.IsApproved = IsApproved;
            operation.Sum = Sum;
            operation.ChildrenOperations = GetAllChildOperations(operation);

            return operation;
        }

        public virtual OperationEntity FromModel(OrderOperation operation, PrimaryKeyResolvingMap pkMap)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            pkMap.AddPair(operation, this);

            Id = operation.Id;
            CreatedDate = operation.CreatedDate;
            CreatedBy = operation.CreatedBy;
            ModifiedDate = operation.ModifiedDate;
            ModifiedBy = operation.ModifiedBy;
            OuterId = operation.OuterId;

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

        public virtual void Patch(OperationEntity target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.Comment = Comment;
            target.Currency = Currency;
            target.Number = Number;
            target.Status = Status;
            target.IsCancelled = IsCancelled;
            target.CancelledDate = CancelledDate;
            target.CancelReason = CancelReason;
            target.IsApproved = IsApproved;

            if (NeedPatchSum)
            {
                target.Sum = Sum;
            }
        }

        private static IEnumerable<IOperation> GetAllChildOperations(IOperation operation)
        {
            var retVal = new List<IOperation>();
            var objectType = operation.GetType();

            var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var childOperations = properties.Where(x => x.PropertyType.GetInterface(typeof(IOperation).Name) != null)
                                    .Select(x => (IOperation)x.GetValue(operation)).Where(x => x != null).ToList();

            foreach (var childOperation in childOperations)
            {
                retVal.Add(childOperation);
            }

            //Handle collection and arrays
            var collections = properties.Where(p => p.GetIndexParameters().Length == 0)
                                        .Select(x => x.GetValue(operation, null))
                                        .Where(x => x is IEnumerable && !(x is string))
                                        .Cast<IEnumerable>();

            foreach (var collection in collections)
            {
                foreach (var childOperation in collection.OfType<IOperation>())
                {
                    retVal.Add(childOperation);
                }
            }

            return retVal;
        }        
    }
}
