using System;

namespace VirtoCommerce.TaxModule.Core.Model
{
    public interface ITaxProviderRegistrar
    {
        void RegisterTaxProvider<T>(Func<T> factory = null) where T : TaxProvider;
    }
}
