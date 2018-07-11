using System;

namespace VirtoCommerce.CoreModule.Core.Tax
{
    public interface ITaxProviderRegistrar
    {
        TaxProvider[] GetAllTaxProviders();
        void RegisterTaxProvider(Func<TaxProvider> providerFactory);
    }
}
