using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Model.Tax;

namespace VirtoCommerce.CoreModule.Core.Services
{
	public class TaxServiceImpl : ITaxService
	{
		private List<Func<TaxProvider>> _taxProviderFactories = new List<Func<TaxProvider>>();

        #region ITaxService Members

        public TaxProvider[] GetAllTaxProviders()
		{
			return _taxProviderFactories.Select(x => x()).ToArray();
		}

		public void RegisterTaxProvider(Func<TaxProvider> providerFactory)
		{
			if (providerFactory == null)
			{
				throw new ArgumentNullException("providerFactory");
			}
            _taxProviderFactories.Add(providerFactory);
		}

		#endregion
	}
}
