using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Modularity.Exceptions;

namespace VirtoCommerce.Platform.Modules
{
    public class LocalStorageModuleCatalog : ModuleCatalog, ILocalModuleCatalog
    {
        private readonly LocalStorageModuleCatalogOptions _options;
        public LocalStorageModuleCatalog(IOptions<LocalStorageModuleCatalogOptions> options)
        {
            _options = options.Value;
            if (_options.VirtualPath != null)
            {
                _options.VirtualPath = _options.VirtualPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
        }

        protected override void InnerLoad()
        {
            var discoveryPath = _options.DiscoveryPath;

            if (string.IsNullOrEmpty(_options.ProbingPath))
                throw new InvalidOperationException("The ProbingPath cannot contain a null value or be empty");
            if (string.IsNullOrEmpty(_options.VirtualPath))
                throw new InvalidOperationException("The VirtualPath cannot contain a null value or be empty");
            if (string.IsNullOrEmpty(_options.DiscoveryPath))
                throw new InvalidOperationException("The DiscoveryPath cannot contain a null value or be empty");

            if (!Directory.Exists(_options.ProbingPath))
            {
                Directory.CreateDirectory(_options.ProbingPath);
            }

            if (!discoveryPath.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
                discoveryPath += "\\";

            var rootUri = new Uri(discoveryPath);

            CopyAssemblies(_options.DiscoveryPath, _options.ProbingPath);

            foreach (var pair in GetModuleManifests())
            {
                var manifest = pair.Value;
                var manifestPath = pair.Key;

                var modulePath = Path.GetDirectoryName(manifestPath);

                CopyAssemblies(modulePath, _options.ProbingPath);

                var moduleVirtualPath = GetModuleVirtualPath(rootUri, modulePath);
                ConvertVirtualPath(manifest.Scripts, moduleVirtualPath);
                ConvertVirtualPath(manifest.Styles, moduleVirtualPath);

                var moduleInfo = new ManifestModuleInfo(manifest) { FullPhysicalPath = Path.GetDirectoryName(manifestPath) };

                // Modules without assembly file don't need initialization
                if (string.IsNullOrEmpty(manifest.AssemblyFile))
                {
                    moduleInfo.State = ModuleState.Initialized;
                }
                else
                {
                    //Set module assembly physical path for future loading by IModuleTypeLoader instance 
                    moduleInfo.Ref = GetFileAbsoluteUri(_options.ProbingPath, manifest.AssemblyFile);
                }

                moduleInfo.IsInstalled = true;
                AddModule(moduleInfo);
            }
        }

        public override IEnumerable<ModuleInfo> CompleteListWithDependencies(IEnumerable<ModuleInfo> modules)
        {
            IEnumerable<ModuleInfo> result;

            try
            {
                var passedModules = modules as ModuleInfo[] ?? modules.ToArray();
                result = base.CompleteListWithDependencies(passedModules).ToArray();
            }
            catch (MissedModuleException)
            {
                // Do not throw if module was missing
                // Use ValidateDependencyGraph to validate & write and error of module missing
                result = Enumerable.Empty<ModuleInfo>();
            }

            return result;
        }

        protected override void ValidateDependencyGraph()
        {
            var modules = Modules.OfType<ManifestModuleInfo>();
            var manifestModules = modules as ManifestModuleInfo[] ?? modules.ToArray();
            try
            {
                base.ValidateDependencyGraph();
            }
            catch (MissedModuleException exception)
            {
                foreach (var module in manifestModules)
                {
                    if (exception.MissedDependenciesMatrix.Keys.Contains(module.ModuleName))
                    {
                        module.Errors.Add($"A module declared a dependency on another module which is not declared to be loaded. Missing module(s): {string.Join(", ", exception.MissedDependenciesMatrix[module.ModuleName])}");
                    }
                }
            }
        }

        public override void Validate()
        {
            var modules = Modules.OfType<ManifestModuleInfo>();
            var manifestModules = modules as ManifestModuleInfo[] ?? modules.ToArray();

            base.Validate();

            //Dependencies and platform version validation
            foreach (var module in manifestModules)
            {
                //Check platform version
                if (!module.PlatformVersion.IsCompatibleWith(PlatformVersion.CurrentVersion))
                {
                    module.Errors.Add($"Module platform version {module.PlatformVersion} is incompatible with current {PlatformVersion.CurrentVersion}");
                }

                //Check that incompatible modules does not installed
                if (!module.Incompatibilities.IsNullOrEmpty())
                {
                    var installedIncompatibilities = manifestModules.Select(x => x.Identity).Join(module.Incompatibilities, x => x.Id, y => y.Id, (x, y) => new { x, y })
                                                            .Where(g => g.y.Version.IsCompatibleWith(g.x.Version)).Select(g => g.x)
                                                            .ToArray();
                    if (installedIncompatibilities.Any())
                    {
                        module.Errors.Add($"{ module } is incompatible with installed { string.Join(", ", installedIncompatibilities.Select(x => x.ToString())) }. You should uninstall these modules first.");
                    }
                }

                foreach (var declaredDependency in module.Dependencies)
                {
                    var installedDependency = manifestModules.FirstOrDefault(x => x.Id.EqualsInvariant(declaredDependency.Id));
                    if (installedDependency != null && !declaredDependency.Version.IsCompatibleWithBySemVer(installedDependency.Version))
                    {
                        module.Errors.Add($"Module dependency {declaredDependency} is incompatible with installed {installedDependency.Version}");
                    }
                }
            }
        }


        private IDictionary<string, ModuleManifest> GetModuleManifests()
        {
            var result = new Dictionary<string, ModuleManifest>();

            if (Directory.Exists(_options.DiscoveryPath))
            {
                foreach (var manifestFile in Directory.EnumerateFiles(_options.DiscoveryPath, "module.manifest", SearchOption.AllDirectories))
                {
                    var manifest = ManifestReader.Read(manifestFile);
                    result.Add(manifestFile, manifest);
                }
            }
            return result;
        }

        private void CopyAssemblies(string sourceParentPath, string targetDirectoryPath)
        {
            if (sourceParentPath != null)
            {
                var sourceDirectoryPath = Path.Combine(sourceParentPath, "bin\\");

                if (Directory.Exists(sourceDirectoryPath))
                {
                    foreach (var sourceFilePath in Directory.EnumerateFiles(sourceDirectoryPath, "*.*", SearchOption.AllDirectories))
                    {
                        if (IsAssemblyFile(sourceFilePath))
                        {
                            var targetFilePath = Path.Combine(targetDirectoryPath, Path.GetFileName(sourceFilePath));
                            CopyFile(sourceFilePath, targetFilePath);
                        }
                    }
                }
            }
        }

        private static void CopyFile(string sourceFilePath, string targetFilePath)
        {
            var sourceFileInfo = new FileInfo(sourceFilePath);
            var targetFileInfo = new FileInfo(targetFilePath);

            var sourceFileVersionInfo = FileVersionInfo.GetVersionInfo(sourceFilePath);
            var sourceVersion = new Version(sourceFileVersionInfo.FileMajorPart, sourceFileVersionInfo.FileMinorPart, sourceFileVersionInfo.FileBuildPart, sourceFileVersionInfo.FilePrivatePart);
            var targetVersion = sourceVersion;

            if (targetFileInfo.Exists)
            {
                var targetFileVersionInfo = FileVersionInfo.GetVersionInfo(targetFilePath);
                targetVersion = new Version(targetFileVersionInfo.FileMajorPart, targetFileVersionInfo.FileMinorPart, targetFileVersionInfo.FileBuildPart, targetFileVersionInfo.FilePrivatePart);
            }

            if (!targetFileInfo.Exists || sourceVersion > targetVersion || (sourceVersion == targetVersion && targetFileInfo.LastWriteTimeUtc < sourceFileInfo.LastWriteTimeUtc))
            {
                var targetDirectoryPath = Path.GetDirectoryName(targetFilePath);
                Directory.CreateDirectory(targetDirectoryPath);
                File.Copy(sourceFilePath, targetFilePath, true);
            }
        }

        private bool IsAssemblyFile(string path)
        {
            var fileExtension = Path.GetExtension(path);
            return fileExtension != null &&  _options.AssemblyFileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        private string GetModuleVirtualPath(Uri rootUri, string modulePath)
        {
            var moduleRelativePath = MakeRelativePath(rootUri, modulePath);
            var moduleVirtualPath = string.Join("/", _options.VirtualPath, moduleRelativePath);

            return moduleVirtualPath;
        }

        private static string MakeRelativePath(Uri rootUri, string fullPath)
        {
            var fullUri = new Uri(fullPath);
            var relativePath = rootUri.MakeRelativeUri(fullUri).ToString();
            return relativePath;
        }

        private static void ConvertVirtualPath(IEnumerable<ManifestBundleItem> items, string moduleVirtualPath)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    const string moduleRoot = "$/";
                    if (item.VirtualPath.StartsWith(moduleRoot, StringComparison.OrdinalIgnoreCase))
                        item.VirtualPath = string.Join("/", moduleVirtualPath, item.VirtualPath.Substring(moduleRoot.Length));
                }
            }
        }

        private static string GetFileAbsoluteUri(string rootPath, string relativePath)
        {
            var builder = new UriBuilder
            {
                Host = string.Empty,
                Scheme = Uri.UriSchemeFile,
                Path = Path.GetFullPath(Path.Combine(rootPath, relativePath))
            };

            return builder.Uri.ToString();
        }
    }
}
