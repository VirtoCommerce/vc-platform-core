using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.Platform.Modules.Abstractions
{
    /// <summary>
    /// Defines the contract for the modules deployed in the application.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Contain module manifest information
        /// </summary>
        ManifestModuleInfo ModuleInfo { get; set; }

        /// <summary>
        /// Allows module to configure database.
        /// This method is called before Initialize().
        /// </summary>
        void SetupDatabase();

        /// <summary>
        /// Notifies the module that it has been initialized.
        /// </summary>
        void Initialize(IServiceCollection serviceCollection);

        /// <summary>
        /// This method is called after all modules have been initialized with Initialize().
        /// </summary>
        void PostInitialize(IServiceCollection serviceCollection);

        /// <summary>
        /// This method is called before uninstalling the module.
        /// </summary>
        void Uninstall();
    }
}
