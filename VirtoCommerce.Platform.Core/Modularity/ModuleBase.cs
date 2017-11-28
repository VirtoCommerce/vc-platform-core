using System;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.Platform.Core.Modularity
{
    public abstract class ModuleBase : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public virtual void SetupDatabase()
        {
        }

        public virtual void Initialize(IServiceCollection serviceCollection)
        {
        }

        public virtual void PostInitialize(IServiceProvider serviceProvider)
        {
        }

        public void Uninstall()
        {
        }
    }
}
