using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Web.TagHelpers
{
    public abstract class ModulesBundleTagHelperBase : TagHelper
    {
        private readonly ILocalModuleCatalog _localModuleCatalog;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly PhysicalFilesWatcher _fileSystemWatcher;
        private readonly LocalStorageModuleCatalogOptions _localStorageModuleCatalogOptions;

        public ModulesBundleTagHelperBase(ILocalModuleCatalog localModuleCatalog, IOptions<LocalStorageModuleCatalogOptions> options, IPlatformMemoryCache platformMemoryCache)
        {
            _localModuleCatalog = localModuleCatalog;
            _platformMemoryCache = platformMemoryCache;
            _localStorageModuleCatalogOptions = options.Value;
            var rootPath = _localStorageModuleCatalogOptions.DiscoveryPath.TrimEnd('\\') + '\\';
            _fileSystemWatcher = new PhysicalFilesWatcher(rootPath, new FileSystemWatcher(rootPath), false);
        }

        [HtmlAttributeName("asp-append-version")]
        public bool AppendVersion { get; set; }

        [HtmlAttributeName("bundle-path")]
        public string BundlePath { get; set; }

        protected abstract TagBuilder GetTagBuilder(string bundleVirtualPath, string version);

        protected string GetBundleVersion(string bundlePhysicalPath)
        {
            if (bundlePhysicalPath == null)
            {
                throw new ArgumentNullException(nameof(bundlePhysicalPath));
            }

            var cacheKey = CacheKey.With(GetType(), "GetBundleVersion", bundlePhysicalPath);
            return _platformMemoryCache.GetOrCreateExclusive(cacheKey, cacheEntry =>
            {
                cacheEntry.AddExpirationToken(_fileSystemWatcher.CreateFileChangeToken(GetRelativePath(bundlePhysicalPath)));
                using (var stream = File.OpenRead(bundlePhysicalPath))
                {
                    var hashAlgorithm = CryptoConfig.AllowOnlyFipsAlgorithms ? (SHA256)new SHA256CryptoServiceProvider() : new SHA256Managed();
                    return $"{WebEncoders.Base64UrlEncode(hashAlgorithm.ComputeHash(stream))}";
                }
            });
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            var sucesfullyLoadedModules = _localModuleCatalog.Modules.OfType<ManifestModuleInfo>().Where(x => x.Errors.IsNullOrEmpty());
            foreach (var module in sucesfullyLoadedModules)
            {
                var normalizedBundlePath = BundlePath.Replace("~/", "").Replace("\\", "/").TrimStart('/');
                var moduleBundleVirtualPath = $"/Modules/$({module.ModuleName})/{normalizedBundlePath}";
                var bundlePhysicalPath = Path.Combine(module.FullPhysicalPath, normalizedBundlePath);
                if (File.Exists(bundlePhysicalPath))
                {
                    string version = null;
                    if (AppendVersion)
                    {
                        version = GetBundleVersion(bundlePhysicalPath);
                    }
                    var tagBuilder = GetTagBuilder(moduleBundleVirtualPath, version);
                    output.Content.AppendHtml(tagBuilder);
                    output.Content.AppendHtml(Environment.NewLine);
                }
            }
        }

        protected virtual string GetRelativePath(string path)
        {
            return path.Replace(_localStorageModuleCatalogOptions.DiscoveryPath, string.Empty).Replace(Path.DirectorySeparatorChar, '/').TrimStart('/');
        }

    }
}
