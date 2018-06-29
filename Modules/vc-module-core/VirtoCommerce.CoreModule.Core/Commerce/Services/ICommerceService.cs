using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Commerce.Model;
using VirtoCommerce.Domain.Commerce.Model;

namespace VirtoCommerce.CoreModule.Core.Commerce.Services
{
	public interface ICommerceService
	{
        IEnumerable<Currency> GetAllCurrencies();
        void UpsertCurrencies(Currency[] currencies);
        void DeleteCurrencies(string[] codes);
        IEnumerable<SeoInfo> GetAllSeoDuplicates();
        IEnumerable<SeoInfo> GetSeoByKeyword(string keyword);
        void UpsertSeoInfos(SeoInfo[] seoinfos);
        void LoadSeoForObjects(ISeoSupport[] seoSupportObjects);
        void UpsertSeoForObjects(ISeoSupport[] seoSupportObjects);
        void DeleteSeoForObject(ISeoSupport seoSupportObject);

        IEnumerable<PackageType> GetAllPackageTypes();
        void UpsertPackageTypes(PackageType[] packageTypes);
        void DeletePackageTypes(string[] ids);

    }
}
