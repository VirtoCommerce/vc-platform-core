using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Smidge;
using Smidge.Models;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Modules.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseModules(this IApplicationBuilder appBuilder)
        {         
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var moduleManager = serviceScope.ServiceProvider.GetRequiredService<IModuleManager>();
                var modules = GetInstalledModules(serviceScope.ServiceProvider);
                foreach (var module in modules)
                {
                    moduleManager.PostInitializeModule(module, serviceScope.ServiceProvider);
                }
            }
            return appBuilder;
        }

        public static IApplicationBuilder UseModulesContent(this IApplicationBuilder appBuilder, IBundleManager bundles)
        {
            var hostingEnv = appBuilder.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            var modules = GetInstalledModules(appBuilder.ApplicationServices);

            var cssBundleItems = modules.SelectMany(m => m.Styles).ToArray();
            
            var cssFiles = cssBundleItems.OfType<ManifestBundleFile>().Select(x => new CssFile(x.VirtualPath));

            cssFiles = cssFiles.Concat(cssBundleItems.OfType<ManifestBundleDirectory>().SelectMany(x => new WebFileFolder(hostingEnv, x.VirtualPath)
                                                                                .AllWebFiles<CssFile>(x.SearchPattern, x.SearchSubdirectories ?  SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));

            var scriptBundleItems = modules.SelectMany(m => m.Scripts).ToArray();
            var jsFiles = scriptBundleItems.OfType<ManifestBundleFile>().Select(x => new JavaScriptFile(x.VirtualPath));
            jsFiles = jsFiles.Concat(scriptBundleItems.OfType<ManifestBundleDirectory>().SelectMany(x => new WebFileFolder(hostingEnv, x.VirtualPath)
                                                                                .AllWebFiles<JavaScriptFile>(x.SearchPattern, x.SearchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));


            //TODO: Test minification and uglification for resulting bundles
            var options = bundles.DefaultBundleOptions;
            options.DebugOptions.FileWatchOptions.Enabled = true;
        
            bundles.Create("vc-modules-styles", cssFiles.ToArray())
               .WithEnvironmentOptions(options);

            bundles.Create("vc-modules-scripts", jsFiles.ToArray())
                   .WithEnvironmentOptions(options);
                          

            return appBuilder;
        }

        private static IEnumerable<ManifestModuleInfo> GetInstalledModules(IServiceProvider serviceProvider)
        {
            var moduleCatalog = serviceProvider.GetRequiredService<ILocalModuleCatalog>();
            var allModules = moduleCatalog.Modules.OfType<ManifestModuleInfo>().ToArray();
            return moduleCatalog.CompleteListWithDependencies(allModules)
                .Where(x => x.State == ModuleState.Initialized)
                .OfType<ManifestModuleInfo>()
                .ToArray();
        }
    }

    /// <summary>
    /// Workaround  suggested by @josh-sachs  https://github.com/Shazwazza/Smidge/issues/47
    /// to allow use recursive directory search for content files
    /// </summary>
    internal class WebFileFolder
    {
        private readonly IHostingEnvironment _env;
        private readonly string _path;

        public WebFileFolder(IHostingEnvironment env, string path)
        {
            _env = env;
            _path = path;
        }

        public T[] AllWebFiles<T>(string pattern, SearchOption search) where T : IWebFile, new()
        {
            var fsPath = _path.Replace("~", _env.WebRootPath);
            return Directory.GetFiles(fsPath, pattern, search)
                .Select(f => new T
                {
                    FilePath = f.Replace(_env.WebRootPath, "~").Replace("\\", "/")
                }).ToArray();
        }
    }
}
