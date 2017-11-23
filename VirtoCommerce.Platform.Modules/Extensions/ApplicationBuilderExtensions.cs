using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Smidge;
using Smidge.Cache;
using Smidge.Models;
using Smidge.Options;
using VirtoCommerce.Platform.Modules.Abstractions;

namespace VirtoCommerce.Platform.Modules.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseModulesContent(this IApplicationBuilder appBuilder, IBundleManager bundles)
        {
            var hostingEnv = appBuilder.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            var moduleCatalog = appBuilder.ApplicationServices.GetRequiredService<ILocalModuleCatalog>();
            var allModules = moduleCatalog.Modules.OfType<ManifestModuleInfo>().ToArray();
            var manifestModules = moduleCatalog.CompleteListWithDependencies(allModules)
                .Where(x => x.State == ModuleState.Initialized)
                .OfType<ManifestModuleInfo>()
                .ToArray();

            var cssBundleItems = manifestModules.SelectMany(m => m.Styles).ToArray();
            
            var cssFiles = cssBundleItems.OfType<ManifestBundleFile>().Select(x => new CssFile(x.VirtualPath));

            cssFiles = cssFiles.Concat(cssBundleItems.OfType<ManifestBundleDirectory>().SelectMany(x => new WebFileFolder(hostingEnv, x.VirtualPath)
                                                                                .AllWebFiles<CssFile>(x.SearchPattern, x.SearchSubdirectories ?  SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));

            var scriptBundleItems = manifestModules.SelectMany(m => m.Scripts).ToArray();
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
