using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Commerce.Model;

namespace VirtoCommerce.CoreModule.Core.Commerce.Services
{
	public interface ICommerceService
	{
        IEnumerable<Currency> GetAllCurrencies();
        void UpsertCurrencies(Currency[] currencies);
        void DeleteCurrencies(string[] codes);
        IEnumerable<SeoInfo> GetAllSeoDuplicates();
        IEnumerable<SeoInfo> GetSeoByKeyword(string keyword);
        Task UpsertSeoInfosAsync(SeoInfo[] seoinfos);
        Task LoadSeoForObjectsAsync(ISeoSupport[] seoSupportObjects);
        Task UpsertSeoForObjectsAsync(ISeoSupport[] seoSupportObjects);
        Task DeleteSeoForObjectAsync(ISeoSupport seoSupportObject);

        IEnumerable<PackageType> GetAllPackageTypes();
        void UpsertPackageTypes(PackageType[] packageTypes);
        void DeletePackageTypes(string[] ids);

    }
}
