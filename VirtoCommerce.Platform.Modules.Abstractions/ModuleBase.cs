using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.Platform.Modules.Abstractions
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

        public virtual void PostInitialize(IServiceCollection serviceCollection)
        {
        }

        public void Uninstall()
        {
        }
    }
}
