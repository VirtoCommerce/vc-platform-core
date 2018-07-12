using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.SitemapsModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Initialize(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            throw new NotImplementedException();
        }

        public void Uninstall()
        {
            throw new NotImplementedException();
        }
    }
}
