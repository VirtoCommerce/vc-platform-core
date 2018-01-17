using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Web.Extensions;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/platform/localization")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LocalizationController : Controller
    {
        private const string LocalizationFilesFormat = ".json";
        private const string LocalizationFilesFolder = "Localizations";
        private const string InternationalizationFilesFormat = ".js";
        private const string InternationalizationFilesFolder = "js\\i18n\\angular";

        private readonly ILocalModuleCatalog _moduleCatalog;
        private readonly IHostingEnvironment _hostingEnv;

        public LocalizationController(ILocalModuleCatalog moduleCatalog, IHostingEnvironment hostingEnv)
        {
            _moduleCatalog = moduleCatalog;
            _hostingEnv = hostingEnv;
        }

        /// <summary>
        /// Return localization resource
        /// </summary>
        /// <param name="lang">Language of localization resource (en by default)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(object), 200)]
        [AllowAnonymous]
        public ActionResult GetLocalization(string lang = "en")
        {
            var searchPattern = string.Format("{0}.*{1}", lang, LocalizationFilesFormat);
            var files = GetAllLocalizationFiles(searchPattern, LocalizationFilesFolder);
            var result = new JObject();
            foreach (var file in files)
            {
                var part = JObject.Parse(System.IO.File.ReadAllText(file));
                result.Merge(part, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });
            }
            return Ok(result);
        }

        /// <summary>
        /// Return all available locales
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("locales")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        public ActionResult GetLocales()
        {
            var files = GetAllLocalizationFiles("*" + LocalizationFilesFormat, LocalizationFilesFolder);
            var locales = files
                .Select(Path.GetFileName)
                .Select(x => x.Substring(0, x.IndexOf('.'))).Distinct().ToArray();

            return Ok(locales);
        }

        /// <summary>
        /// Return all available regional formats
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("regionalformats")]
        [AllowAnonymous]
        public ActionResult GetRegionalFormats()
        {
            var files = GetAllInternationalizationFiles("*" + InternationalizationFilesFormat, InternationalizationFilesFolder);
            var formats = files
                .Select(Path.GetFileName)
                .Select(x =>
                {
                    var startIndexOfCode = x.IndexOf("_") + 1;
                    var endIndexOfCode = x.IndexOf(".");
                    return x.Substring(startIndexOfCode, endIndexOfCode - startIndexOfCode);
                }).Distinct().ToArray();

            return Ok(formats);
        }

        private string[] GetAllLocalizationFiles(string searchPattern, string localizationsFolder)
        {
            var files = new List<string>();

            // Get platform localization files
            var platformPath = _hostingEnv.MapPath("~/");
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
            var userLocalizationPath = _hostingEnv.MapPath("~/App_Data");
            var userFileNames = GetFilesByPath(userLocalizationPath, searchPattern, localizationsFolder);
            files.AddRange(userFileNames);
            return files.ToArray();
        }

        private string[] GetAllInternationalizationFiles(string searchPattern, string internationalizationsFolder)
        {
            var files = new List<string>();

            // Get platform internationalization files
            var platformPath = _hostingEnv.MapPath("~/");
            var platformFileNames = GetFilesByPath(platformPath, searchPattern, internationalizationsFolder);
            files.AddRange(platformFileNames);

            return files.ToArray();
        }

        private string[] GetFilesByPath(string path, string searchPattern, string subfolder)
        {
            var sourceDirectoryPath = Path.Combine(path, subfolder);

            return Directory.Exists(sourceDirectoryPath)
                ? Directory.EnumerateFiles(sourceDirectoryPath, searchPattern, SearchOption.AllDirectories).ToArray()
                : new string[0];
        }
    }
}
