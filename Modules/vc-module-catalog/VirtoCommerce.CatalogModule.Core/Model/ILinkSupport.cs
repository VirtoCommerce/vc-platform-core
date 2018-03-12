using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface ILinkSupport
	{
        IList<CategoryLink> Links { get; set; }
	}
}
