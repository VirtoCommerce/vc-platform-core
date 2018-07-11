using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.CoreModule.Core.Currency
{
    /// <summary>
    /// Represent DAL for currencies 
    /// </summary>
    public interface ICurrencyService
    {
        Task<IEnumerable<Currency>> GetAllCurrenciesAsync();
        Task SaveChangesAsync(Currency[] currencies);
        Task DeleteCurrenciesAsync(string[] codes);
    }
}
