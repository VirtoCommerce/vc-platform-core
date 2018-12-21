using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
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
                    moduleManager.PostInitializeModule(module, appBuilder);
                }
            }
            return appBuilder;
        }

        public static IApplicationBuilder UseModulesContent(this IApplicationBuilder appBuilder, IBundleManager bundles)
        {
            //var env = appBuilder.ApplicationServices.GetService<IHostingEnvironment>();
            //var modules = GetInstalledModules(appBuilder.ApplicationServices);
            //var modulesOptions = appBuilder.ApplicationServices.GetRequiredService<IOptions<LocalStorageModuleCatalogOptions>>().Value;
            //var cssBundleItems = modules.SelectMany(m => m.Styles).ToArray();
            //var cssFiles = cssBundleItems.OfType<ManifestBundleFile>().Select(x => new CssFile(x.VirtualPath));

            //cssFiles = cssFiles.Concat(cssBundleItems.OfType<ManifestBundleDirectory>().SelectMany(x => new WebFileFolder(modulesOptions.DiscoveryPath, x.VirtualPath)
            //                                                                    .AllWebFiles<CssFile>(x.SearchPattern, x.SearchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))).ToArray();

            //var scriptBundleItems = modules.SelectMany(m => m.Scripts).ToArray();
            //var jsFiles = scriptBundleItems.OfType<ManifestBundleFile>().Select(x => new JavaScriptFile(x.VirtualPath));

            //jsFiles = modules.Aggregate(jsFiles, (current, module) =>
            //{
            //    return current.Concat(module.Scripts
            //        .OfType<ManifestBundleDirectory>()
            //        .SelectMany(s =>
            //            env.IsDevelopment() ?
            //                new WebFileFolder(modulesOptions.DiscoveryPath, s.VirtualPath, module.ModuleName, module.Assembly.GetName().Name)
            //                    .AllWebFilesForDevelopment<JavaScriptFile>(s.SearchPattern,
            //                        s.SearchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly) :
            //            new WebFileFolder(modulesOptions.DiscoveryPath, s.VirtualPath)
            //                .AllWebFilesWithRequestRoot<JavaScriptFile>(s.SearchPattern,
            //                    s.SearchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));
            //});

            //var options = bundles.DefaultBundleOptions;
            //options.DebugOptions.FileWatchOptions.Enabled = true;
            //options.DebugOptions.ProcessAsCompositeFile = false;
            //options.DebugOptions.CompressResult = false;
            //options.DebugOptions.CacheControlOptions = new CacheControlOptions() { EnableETag = false, CacheControlMaxAge = 0 };

            //bundles.Create("vc-modules-styles", cssFiles.ToArray())
            //   .WithEnvironmentOptions(options);

            //bundles.Create("vc-modules-scripts", bundles.PipelineFactory.Create<NuglifyJs>(), jsFiles.ToArray())
            //       .WithEnvironmentOptions(options);


            return appBuilder;
        }

        private static IEnumerable<ManifestModuleInfo> GetInstalledModules(IServiceProvider serviceProvider)
        {
            var moduleCatalog = serviceProvider.GetRequiredService<ILocalModuleCatalog>();
            var allModules = moduleCatalog.Modules.OfType<ManifestModuleInfo>().ToArray();
            return moduleCatalog.CompleteListWithDependencies(allModules)
                .OfType<ManifestModuleInfo>()
                .Where(x => x.State == ModuleState.Initialized && !x.Errors.Any())
                .OrderBy(m => m.Id)
                .ToArray();
        }
    }

    /// <summary>
    /// Workaround  suggested by @josh-sachs  https://github.com/Shazwazza/Smidge/issues/47
    /// to allow use recursive directory search for content files
    /// </summary>
    internal class WebFileFolder
    {
        private readonly string _rootPath;
        private readonly string _path;
        private readonly string _moduleName;
        private readonly string _moduleFolder;

        public WebFileFolder(string rootPath, string path)
        {
            _rootPath = rootPath;
            _path = path;
        }

        public WebFileFolder(string rootPath, string path, string moduleName, string moduleFolder)
        {
            _rootPath = rootPath;
            _path = path;
            _moduleName = moduleName;
            _moduleFolder = moduleFolder;
        }

        public T[] AllWebFiles<T>(string pattern, SearchOption search) where T : IWebFile, new()
        {
            var result = Directory.GetFiles(Path.Combine(_rootPath, _path), pattern, search)
                 .Select(f => new T
                 {
                     FilePath = f.Replace(_rootPath, "~").Replace("\\", "/")
                 }).ToArray();
            return result;
        }

        public T[] AllWebFilesForDevelopment<T>(string pattern, SearchOption search) where T : IWebFile, new()
        {
            return Directory.GetFiles(Path.Combine(_rootPath, _path), pattern, search)
                .Select(f =>
                {
                    var result = new T
                    {
                        FilePath = GetRelativePath(f, GetRootFolder(_path))
                    };
                    return result;
                }).ToArray();
        }

        public T[] AllWebFilesWithRequestRoot<T>(string pattern, SearchOption search) where T : IWebFile, new()
        {
            return Directory.GetFiles(Path.Combine(_rootPath, _path), pattern, search)
                .Select(f =>
                {
                    var result = new T
                    {
                        FilePath = f.Replace(_rootPath, "~").Replace("\\", "/"),
                        RequestPath = GetRelativePath(f, GetRootFolder(_path))
                    };
                    return result;
                }).ToArray();
        }

        string GetRelativePath(string fullPath, string moduleFolder)
        {
            return fullPath.Replace(_rootPath, "")
                .Replace(moduleFolder, "Modules")
                .Replace("\\", "/")
                .Replace($"{_moduleFolder}", $"$({_moduleName})");
        }

        string GetRootFolder(string path)
        {
            while (true)
            {
                string temp = Path.GetDirectoryName(path);
                if (String.IsNullOrEmpty(temp))
                    break;
                path = temp;
            }
            return path;
        }
    }
}
