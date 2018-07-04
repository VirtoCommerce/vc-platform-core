using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Commerce.Model;
using VirtoCommerce.CoreModule.Core.Commerce.Services;

namespace VirtoCommerce.CoreModule.Data.Services
{
    public class CommerceService : ICommerceService
    {
        public IEnumerable<Currency> GetAllCurrencies()
        {
            return new List<Currency>()
            {
                new Currency()
                {
                    Code = "USD",
                    Name = "US dollar",
                    IsPrimary = true,
                    ExchangeRate = 1,
                    Symbol = "$",
                }
            };
        }

        public void UpsertCurrencies(Currency[] currencies)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteCurrencies(string[] codes)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<SeoInfo> GetAllSeoDuplicates()
        {
            return Enumerable.Empty<SeoInfo>();
        }

        public IEnumerable<SeoInfo> GetSeoByKeyword(string keyword)
        {
            return Enumerable.Empty<SeoInfo>();
        }

        public Task UpsertSeoInfosAsync(SeoInfo[] seoinfos)
        {
            return Task.CompletedTask;
        }

        public Task LoadSeoForObjectsAsync(ISeoSupport[] seoSupportObjects)
        {
            return Task.CompletedTask;
        }

        public Task UpsertSeoForObjectsAsync(ISeoSupport[] seoSupportObjects)
        {
            return Task.CompletedTask;
        }

        public Task DeleteSeoForObjectAsync(ISeoSupport seoSupportObject)
        {
            return Task.CompletedTask;
        }

        public IEnumerable<PackageType> GetAllPackageTypes()
        {
            throw new System.NotImplementedException();
        }

        public void UpsertPackageTypes(PackageType[] packageTypes)
        {
            throw new System.NotImplementedException();
        }

        public void DeletePackageTypes(string[] ids)
        {
            throw new System.NotImplementedException();
        }
    }
}
