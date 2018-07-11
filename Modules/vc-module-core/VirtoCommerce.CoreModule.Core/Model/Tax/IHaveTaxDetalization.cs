using System.Collections.Generic;

namespace VirtoCommerce.CoreModule.Core.Model.Tax
{
	public interface IHaveTaxDetalization
	{
		ICollection<TaxDetail> TaxDetails { get; set; } 
	}
}
