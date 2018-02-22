using System;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.CoreModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
        }

        public void PostInitialize(IServiceProvider serviceProvider)
        {
        }

        public void Uninstall()
        {
        }
    }
}

