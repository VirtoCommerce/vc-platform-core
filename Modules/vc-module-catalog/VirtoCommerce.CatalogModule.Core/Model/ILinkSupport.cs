using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
	public interface ILinkSupport
	{
		ICollection<CategoryLink> Links { get; set; }
	}
}
