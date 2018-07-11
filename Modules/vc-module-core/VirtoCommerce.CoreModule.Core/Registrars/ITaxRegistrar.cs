using System;
using VirtoCommerce.CoreModule.Core.Model.Tax;

namespace VirtoCommerce.CoreModule.Core.Registrars
{
    /// <summary>
    /// Tax provider factory
    /// </summary>
    public interface ITaxRegistrar
    {
        TaxProvider[] GetAllTaxProviders();
        void RegisterTaxProvider(Func<TaxProvider> providerFactory);
    }
}
