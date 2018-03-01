using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Extensions.Logging;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Common;
namespace VirtoCommerce.Platform.Modules
{

    public class LoadContextAssemblyResolver : IAssemblyResolver
    {
        private readonly ILogger<LoadContextAssemblyResolver> _logger;
        public LoadContextAssemblyResolver(ILogger<LoadContextAssemblyResolver> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Registers the specified assembly and resolves the types in it when the AppDomain requests for it.
        /// </summary>
        /// <param name="assemblyFilePath">The path to the assembly to load in the LoadFrom context.</param>    
        public Assembly LoadAssemblyFrom(string assemblyFilePath)
        {
            Uri assemblyUri = GetFileUri(assemblyFilePath);

            if (assemblyUri == null)
            {
                throw new ArgumentException("The argument must be a valid absolute Uri to an assembly file.", nameof(assemblyFilePath));
            }

            if (!File.Exists(assemblyUri.LocalPath))
            {
                throw new FileNotFoundException(assemblyUri.LocalPath);
            }

            var assembly = LoadWithAllReferencedAssebliesRecursive(assemblyUri.LocalPath);
            return assembly;
        }

        private Assembly LoadWithAllReferencedAssebliesRecursive(string assemblyPath)
        {
            Assembly assembly = null;

            if (File.Exists(assemblyPath))
            {
                // assembly = new AssemblyResolver(assemblyPath, _logger).Assembly;
                assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                var dependencyContext = DependencyContext.Load(assembly);

                var assemblyResolver = new CompositeCompilationAssemblyResolver
                                        (new ICompilationAssemblyResolver[]
                {
                        new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(assemblyPath)),
                        new ReferenceAssemblyPathResolver(),
                        new PackageCompilationAssemblyResolver()
                });
                var loadContext = AssemblyLoadContext.GetLoadContext(assembly);

                //These event handler required for load modules dependencies (packages and libraries). It is the best solution what I've found.
                //https://www.codeproject.com/Articles/1194332/Resolving-Assemblies-in-NET-Core
                //https://github.com/dotnet/corefx/issues/11639
                //https://github.com/dotnet/coreclr/blob/master/Documentation/design-docs/assemblyloadcontext.md
                //https://github.com/dotnet/core-setup/blob/master/Documentation/design-docs/corehost.md
                loadContext.Resolving += (context, assemblyName) =>
                {
                    bool assemblyPredicate(RuntimeLibrary runtime)
                    {
                        var result = runtime.Name.EqualsInvariant(assemblyName.Name);
                        if (result)
                        {
                            //Need to do an additional comparison by version because modules can use different versions
                            if (Version.TryParse(runtime.Version, out Version version))
                            {
                                result = new SemanticVersion(assemblyName.Version).IsCompatibleWith(new SemanticVersion(version));
                            }
                        }
                        return result;
                    }
                    _logger.LogDebug($"Trying to resolve {assemblyName} in the {assembly.GetName().Name} dependencies");

                    var library = dependencyContext.RuntimeLibraries.FirstOrDefault(assemblyPredicate);
                    if (library != null)
                    {
                        var wrapper = new CompilationLibrary(
                            library.Type,
                            library.Name,
                            library.Version,
                            library.Hash,
                            library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                            library.Dependencies,
                            library.Serviceable);

                        var assemblies = new List<string>();
                        assemblyResolver.TryResolveAssemblyPaths(wrapper, assemblies);
                        if (assemblies.Count > 0)
                        {
                            _logger.LogDebug($"Load assembly {assemblyName} from {assemblies[0]}");

                            return context.LoadFromAssemblyPath(assemblies[0]);
                        }
                    }
                    return null;
                };
            }
            return assembly;
        }

        private static Uri GetFileUri(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                return null;
            }

            if (!Uri.TryCreate(filePath, UriKind.Absolute, out Uri uri))
            {
                return null;
            }

            if (!uri.IsFile)
            {
                return null;
            }

            return uri;
        }
    }

}
