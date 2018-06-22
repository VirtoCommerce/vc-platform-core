using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Commerce.Services;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.CoreModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICommerceService, MockCommerceService>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
        }

        public void Uninstall()
        {
        }

        public class MockCommerceService : ICommerceService
        {
            public void DeleteCurrencies(string[] codes)
            {
            }

            public void DeleteFulfillmentCenter(string[] ids)
            {
            }

            public void DeletePackageTypes(string[] ids)
            {
            }

            public void DeleteSeoForObject(ISeoSupport seoSupportObject)
            {
            }

            public IEnumerable<Currency> GetAllCurrencies()
            {
                return Enumerable.Empty<Currency>();
            }

            public IEnumerable<FulfillmentCenter> GetAllFulfillmentCenters()
            {
                return Enumerable.Empty<FulfillmentCenter>();
            }

            public IEnumerable<PackageType> GetAllPackageTypes()
            {
                return Enumerable.Empty<PackageType>();
            }

            public IEnumerable<SeoInfo> GetAllSeoDuplicates()
            {
                return Enumerable.Empty<SeoInfo>();
            }

            public IEnumerable<SeoInfo> GetSeoByKeyword(string keyword)
            {
                return Enumerable.Empty<SeoInfo>();
            }

            public void LoadSeoForObjects(ISeoSupport[] seoSupportObjects)
            {
            }

            public void UpsertCurrencies(Currency[] currencies)
            {
            }

            public FulfillmentCenter UpsertFulfillmentCenter(FulfillmentCenter fullfilmentCenter)
            {
                return fullfilmentCenter;
            }

            public void UpsertPackageTypes(PackageType[] packageTypes)
            {

            }

            public void UpsertSeoForObjects(ISeoSupport[] seoSupportObjects)
            {

            }

            public void UpsertSeoInfos(SeoInfo[] seoinfos)
            {

            }
        }
    }
}

