using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IPropertyService
	{
        IEnumerable<Property> GetByIds(IEnumerable<string> ids);
    	void SaveChanges(IEnumerable<Property> properties);
		void Delete(IEnumerable<string> ids, bool doDeleteValues = false);
  	}
}
