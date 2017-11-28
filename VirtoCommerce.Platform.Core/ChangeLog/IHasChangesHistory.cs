using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.ChangeLog
{
	public interface IHasChangesHistory : IEntity
	{
		ICollection<OperationLog> OperationsLog { get; set; }
	}
}
