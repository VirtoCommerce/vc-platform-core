using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CoreModule.Core.Commerce.Model;
using VirtoCommerce.CoreModule.Core.Commerce.Services;
using VirtoCommerce.CoreModule.Core.Services;
using VirtoCommerce.CoreModule.Data.Services;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.CoreModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICommerceService, CommerceService>();
            serviceCollection.AddSingleton<IShippingMethodsRegistrar>(new ShippingMethodsRegistrarImpl());
            serviceCollection.AddSingleton<IPaymentMethodsRegistrar>(new PaymentMethodsRegistrarImpl());
            serviceCollection.AddSingleton<ITaxRegistrar>(new TaxRegistrarImpl());
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
        }

        public void Uninstall()
        {
        }
    }
}

