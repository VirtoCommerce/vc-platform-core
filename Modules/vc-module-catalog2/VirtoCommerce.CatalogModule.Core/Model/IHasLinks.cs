using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
    public interface IHasLinks : IEntity
	{
        IList<CategoryLink> Links { get; set; }
	}
}
