using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Commerce.Model;
using VirtoCommerce.CoreModule.Core.Commerce.Services;
using VirtoCommerce.Domain.Commerce.Model;

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

        public void UpsertSeoInfos(SeoInfo[] seoinfos)
        {
            throw new System.NotImplementedException();
        }

        public void LoadSeoForObjects(ISeoSupport[] seoSupportObjects)
        {
        }

        public void UpsertSeoForObjects(ISeoSupport[] seoSupportObjects)
        {
        }

        public void DeleteSeoForObject(ISeoSupport seoSupportObject)
        {
            throw new System.NotImplementedException();
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
