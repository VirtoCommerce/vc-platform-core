using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Localizations;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Data.Localizations
{
    public class LocalizationService : ILocalizationService
    {
        private const string LocalizationFilesFormat = ".json";
        private const string LocalizationFilesFolder = "Localizations";

        private readonly ILocalModuleCatalog _moduleCatalog;
        private readonly IHostingEnvironment _hostingEnv;

        private readonly IPlatformMemoryCache _memoryCache;

        public LocalizationService(ILocalModuleCatalog moduleCatalog, IHostingEnvironment hostingEnv, IPlatformMemoryCache memoryCache)
        {
            _moduleCatalog = moduleCatalog;
            _hostingEnv = hostingEnv;
            _memoryCache = memoryCache;
        }

        public object LocalizationResources { get; set; }

        public object GetLocalization(string lang = "en")
        {
            var searchPattern = string.Format("{0}.*{1}", lang, LocalizationFilesFormat);
            var files = GetAllLocalizationFiles(searchPattern, LocalizationFilesFolder);
            var result = new JObject();
            foreach (var file in files)
            {
                var part = JObject.Parse(File.ReadAllText(file));
                result.Merge(part, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });
            }

            return result;
        }

        public string[] GetLocales()
        {
            var files = GetAllLocalizationFiles("*" + LocalizationFilesFormat, LocalizationFilesFolder);
            var locales = files
                .Select(Path.GetFileName)
                .Select(x => x.Substring(0, x.IndexOf('.'))).Distinct().ToArray();

            return locales;
        }

        public void FillLocalizationResources()
        {
            var files = GetAllLocalizationFiles("*" + LocalizationFilesFormat, LocalizationFilesFolder);
            var locales = files.Select(Path.GetFileName).Select(x => x.Substring(0, x.IndexOf('.'))).Distinct().ToArray();
            var result = new JObject();

            foreach (var locale in locales)
            {
                var localizationValues = new JObject();
                foreach (var file in files.Where(f => Path.GetFileName(f).StartsWith(locale)))
                {
                    var fileName = Path.GetFileName(file);
                    var part = JObject.Parse(File.ReadAllText(file));
                    localizationValues.Merge(part, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });
                }
                result.Add(locale, localizationValues);
            }

            LocalizationResources = result;
        }

        private string[] GetAllLocalizationFiles(string searchPattern, string localizationsFolder)
        {
            var cacheKey = CacheKey.With(GetType(), "GetAllLocalizationFiles", searchPattern, localizationsFolder);
            return _memoryCache.GetOrCreateExclusive(cacheKey, cacheEntry =>
            {
                //Add cache  expiration token
                cacheEntry.AddExpirationToken(LocalizationCacheRegion.CreateChangeToken());
                var files = new List<string>();

                // Get platform localization files
                var platformPath = MapPath(_hostingEnv, "~/");
                var platformFileNames = GetFilesByPath(platformPath, searchPattern, localizationsFolder);
                files.AddRange(platformFileNames);

                // Get modules localization files ordered by dependency.
                var allModules = _moduleCatalog.Modules.OfType<ManifestModuleInfo>().ToArray();
                var manifestModules = _moduleCatalog.CompleteListWithDependencies(allModules)
                    .Where(x => x.State == ModuleState.Initialized)
                    .OfType<ManifestModuleInfo>();

                foreach (var module in manifestModules)
                {
                    if (!string.IsNullOrEmpty(module.FullPhysicalPath))
                    {
                        var moduleFileNames = GetFilesByPath(module.FullPhysicalPath, searchPattern, localizationsFolder);
                        files.AddRange(moduleFileNames);
                    }
                }
                // Get user defined localization files from App_Data/Localizations folder
                var userLocalizationPath = MapPath(_hostingEnv, "~/App_Data");
                var userFileNames = GetFilesByPath(userLocalizationPath, searchPattern, localizationsFolder);
                files.AddRange(userFileNames);
                return files.ToArray();
            });
        }

        private string[] GetFilesByPath(string path, string searchPattern, string subfolder)
        {
            var sourceDirectoryPath = Path.Combine(path, subfolder);

            return Directory.Exists(sourceDirectoryPath)
                ? Directory.EnumerateFiles(sourceDirectoryPath, searchPattern, SearchOption.AllDirectories).ToArray()
                : new string[0];
        }

        public string MapPath(IHostingEnvironment hostEnv, string path)
        {
            var result = hostEnv.WebRootPath;

            if (path.StartsWith("~/"))
            {
                result = Path.Combine(result, path.Replace("~/", string.Empty).Replace('/', Path.DirectorySeparatorChar));
            }
            else if (Path.IsPathRooted(path))
            {
                result = path;
            }

            return result;
        }
    }
}
