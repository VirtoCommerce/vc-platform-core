using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using VirtoCommerce.Platform.Core.Modularity;

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

            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyUri.LocalPath);
            foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
            {
                var referencedAssemblyPath = Path.Combine(Path.GetDirectoryName(assemblyUri.LocalPath), referencedAssembly.Name + ".dll");
                if (File.Exists(referencedAssemblyPath))
                {
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(referencedAssemblyPath);
                }
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
