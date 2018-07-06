using System;

namespace VirtoCommerce.CoreModule.Core.Model
{
	public interface ISupportCancellation
	{
		bool IsCancelled { get; set; }
		DateTime? CancelledDate { get; set; }
		string CancelReason { get; set; }
	}
}
