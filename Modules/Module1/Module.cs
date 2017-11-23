using Microsoft.Extensions.DependencyInjection;
using Module1.Abstractions;
using Module1.Services;
using System;
using VirtoCommerce.Platform.Modules.Abstractions;

namespace Module1
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMyService, MyServiceImpl>();
        }

        public void PostInitialize(IServiceCollection serviceCollection)
        {
        }

        public void SetupDatabase()
        {
        }

        public void Uninstall()
        {
        }
    }
}
