using System.Collections.Generic;

namespace VirtoCommerce.CoreModule.Core.Commerce.Model
{
	public interface IHaveTaxDetalization
	{
		ICollection<TaxDetail> TaxDetails { get; set; } 
	}
}
