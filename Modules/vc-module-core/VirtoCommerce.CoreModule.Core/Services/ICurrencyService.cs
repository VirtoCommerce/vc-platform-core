using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Model;

namespace VirtoCommerce.CoreModule.Core.Services
{
    public interface ICurrencyService
    {
        Task<IEnumerable<Currency>> GetAllCurrenciesAsync();
        Task UpsertCurrenciesAsync(Currency[] currencies);
        Task DeleteCurrenciesAsync(string[] codes);
    }
}
