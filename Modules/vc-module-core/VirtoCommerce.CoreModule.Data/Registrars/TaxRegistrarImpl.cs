using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Model.Tax;
using VirtoCommerce.CoreModule.Core.Registrars;

namespace VirtoCommerce.CoreModule.Data.Registrars
{
	public class TaxRegistrarImpl : ITaxRegistrar
	{
		private List<Func<TaxProvider>> _taxProviderFactories = new List<Func<TaxProvider>>();

        #region ITaxRegistrar Members

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
