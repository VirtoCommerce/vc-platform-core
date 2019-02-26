using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core2.Model;

namespace VirtoCommerce.CatalogModule.Core2.Services
{
    public interface IPropertyService
	{
        IEnumerable<Property> GetByIds(IEnumerable<string> ids);
    	void SaveChanges(IEnumerable<Property> properties);
		void Delete(IEnumerable<string> ids, bool doDeleteValues = false);
  	}
}
