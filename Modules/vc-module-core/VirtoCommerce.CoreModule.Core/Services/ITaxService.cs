using System;
using VirtoCommerce.CoreModule.Core.Model.Tax;

namespace VirtoCommerce.CoreModule.Core.Services
{
    /// <summary>
    /// Tax provider factory
    /// </summary>
    public interface ITaxService
    {
        TaxProvider[] GetAllTaxProviders();
        void RegisterTaxProvider(Func<TaxProvider> providerFactory);
    }
}
