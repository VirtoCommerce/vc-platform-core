using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Common
{
    public interface IOperation : IEntity
    {
        string OperationType { get; set; }
        string Number { get; set; }
        bool IsApproved { get; set; }
        string Status { get; set; }

        string Comment { get; set; }
        string Currency { get; set; }

        string ParentOperationId { get; set; }

        IEnumerable<IOperation> ChildrenOperations { get; set; }
    }
}
