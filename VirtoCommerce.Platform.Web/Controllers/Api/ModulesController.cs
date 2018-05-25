using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Helpers;
using VirtoCommerce.Platform.Modules;
using VirtoCommerce.Platform.Web.Extensions;
using VirtoCommerce.Platform.Web.Model.Modularity;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/platform/modules")]
    [Authorize(SecurityConstants.Permissions.ModuleAccess)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ModulesController : Controller
    {
        private const string _uploadsUrl = "~/App_Data/Uploads/";
        private readonly IExternalModuleCatalog _moduleCatalog;
        private readonly IModuleInstaller _moduleInstaller;
        private readonly IPushNotificationManager _pushNotifier;
        private readonly IUserNameResolver _userNameResolver;
        private readonly Core.Settings.ISettingsManager _settingsManager;
        private readonly IHostingEnvironment _hostingEnv;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        private readonly ExternalModuleCatalogOptions _extModuleOptions;
        private static readonly object _lockObject = new object();

        public ModulesController(IExternalModuleCatalog moduleCatalog, IModuleInstaller moduleInstaller, IPushNotificationManager pushNotifier,
            IUserNameResolver userNameResolver, IHostingEnvironment hostingEnv, Core.Settings.ISettingsManager settingsManager,
            IOptions<ExternalModuleCatalogOptions> extModuleOptions)
        {
            _moduleCatalog = moduleCatalog;
            _moduleInstaller = moduleInstaller;
            _pushNotifier = pushNotifier;
            _userNameResolver = userNameResolver;
            _settingsManager = settingsManager;
            _hostingEnv = hostingEnv;
            _extModuleOptions = extModuleOptions.Value;
        }

        /// <summary>
        /// Reload  modules
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("reload")]
        [Authorize(SecurityConstants.Permissions.ModuleQuery)]
        public ActionResult ReloadModules()
        {
            _moduleCatalog.Reload();
            return NoContent();
        }

        /// <summary>
        /// Get installed modules
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Authorize(SecurityConstants.Permissions.ModuleQuery)]
        public ActionResult GetModules()
        {
            EnsureModulesCatalogInitialized();

            var retVal = _moduleCatalog.Modules
                                       .OfType<ManifestModuleInfo>().OrderBy(x => x.Id).ThenBy(x => x.Version)
                                       .Select(x => AbstractTypeFactory<ModuleDescriptor>.TryCreateInstance().FromModel(x))
                                       .ToArray();

            return Ok(retVal);
        }

        /// <summary>
        /// Get all dependent modules for module
        /// </summary>
        /// <param name="moduleDescriptors">modules descriptors</param>
        /// <returns></returns>
        [HttpPost]
        [Route("getdependents")]
        [Authorize(SecurityConstants.Permissions.ModuleQuery)]
        public ActionResult GetDependingModules([FromBody] ModuleDescriptor[] moduleDescriptors)
        {
            EnsureModulesCatalogInitialized();

            var modules = _moduleCatalog.Modules
                .OfType<ManifestModuleInfo>()
                .Join(moduleDescriptors, x => x.Identity, y => y.Identity, (x, y) => x)
                .ToList();

            var retVal = GetDependingModulesRecursive(modules).Distinct()
                                                              .Except(modules)
                                                              .Select(x => AbstractTypeFactory<ModuleDescriptor>.TryCreateInstance().FromModel(x))
                                                              .ToArray();
            return Ok(retVal);
        }

        /// <summary>
        /// Returns a flat expanded  list of modules that depend on passed modules
        /// </summary>
        /// <param name="moduleDescriptors">modules descriptors</param>
        /// <returns></returns>
        [HttpPost]
        [Route("getmissingdependencies")]
        [Authorize(SecurityConstants.Permissions.ModuleQuery)]
        public ActionResult GetMissingDependencies([FromBody] ModuleDescriptor[] moduleDescriptors)
        {
            EnsureModulesCatalogInitialized();
            var modules = _moduleCatalog.Modules
                                        .OfType<ManifestModuleInfo>().Join(moduleDescriptors, x => x.Identity, y => y.Identity, (x, y) => x)
                                        .ToList();

            var retVal = _moduleCatalog.CompleteListWithDependencies(modules)
                                       .OfType<ManifestModuleInfo>()
                                       .Where(x => !x.IsInstalled)
                                       .Except(modules)
                                       .Select(x => AbstractTypeFactory<ModuleDescriptor>.TryCreateInstance().FromModel(x))
                                       .ToArray();

            return Ok(retVal);
        }

        /// <summary>
        /// Upload module package for installation or update
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("localstorage")]
        [Authorize(SecurityConstants.Permissions.ModuleManage)]
        public async Task<ActionResult> UploadModuleArchive()
        {
            ModuleDescriptor result = null;

            EnsureModulesCatalogInitialized();

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            string targetFilePath = null;

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType),  _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            if (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        //ToDo After update to core 2.1 make beautiful https://github.com/aspnet/HttpAbstractions/issues/446
                        var fileName = contentDisposition.FileName.Value.TrimStart('\"').TrimEnd('\"');

                        targetFilePath = Path.Combine(_hostingEnv.MapPath(_uploadsUrl), fileName);
                        using (var targetStream = System.IO.File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                        }
                    }
                }
            }

            using (var packageStream = System.IO.File.Open(targetFilePath, FileMode.Open))
            using (var package = new ZipArchive(packageStream, ZipArchiveMode.Read))
            {
                var entry = package.GetEntry("module.manifest");
                if (entry != null)
                {
                    using (var manifestStream = entry.Open())
                    {
                        var manifest = ManifestReader.Read(manifestStream);
                        var module = new ManifestModuleInfo(manifest);
                        var alreadyExistModule = _moduleCatalog.Modules.OfType<ManifestModuleInfo>().FirstOrDefault(x => x.Equals(module));
                        if (alreadyExistModule != null)
                        {
                            module = alreadyExistModule;
                        }
                        else
                        {
                            //Force dependency validation for new module
                            _moduleCatalog.CompleteListWithDependencies(new[] { module }).ToList().Clear();
                            _moduleCatalog.AddModule(module);
                        }

                        module.Ref = targetFilePath;
                        result = AbstractTypeFactory<ModuleDescriptor>.TryCreateInstance().FromModel(module);
                    }
                }
            }
            return Ok(result);
        }

        /// <summary>
        /// Install modules 
        /// </summary>
        /// <param name="modules">modules for install</param>
        /// <returns></returns>
        [HttpPost]
        [Route("install")]
        [Authorize(SecurityConstants.Permissions.ModuleManage)]
        public ActionResult InstallModules([FromBody] ModuleDescriptor[] modules)
        {
            EnsureModulesCatalogInitialized();

            var options = new ModuleBackgroundJobOptions
            {
                Action = ModuleAction.Install,
                Modules = modules
            };
            var result = ScheduleJob(options);
            return Ok(result);
        }

        /// <summary>
        /// Uninstall module
        /// </summary>
        /// <param name="modules">modules</param>
        /// <returns></returns>
        [HttpPost]
        [Route("uninstall")]
        [Authorize(SecurityConstants.Permissions.ModuleManage)]
        public ActionResult UninstallModule([FromBody] ModuleDescriptor[] modules)
        {
            EnsureModulesCatalogInitialized();

            var options = new ModuleBackgroundJobOptions
            {
                Action = ModuleAction.Uninstall,
                Modules = modules
            };
            var result = ScheduleJob(options);
            return Ok(result);
        }

        /// <summary>
        /// Restart web application
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("restart")]
        [Authorize(SecurityConstants.Permissions.ModuleManage)]
        public ActionResult Restart()
        {
            //TODO:
            throw new NotImplementedException();
        }

        /// <summary>
        /// Auto-install modules with specified groups
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("autoinstall")]
        [Authorize(SecurityConstants.Permissions.PlatformImport)]
        public ActionResult TryToAutoInstallModules()
        {
            var notification = new ModuleAutoInstallPushNotification(User.Identity.Name)
            {
                Title = "Modules installation",
                //set completed by default
                Finished = DateTime.UtcNow
            };


            if (!_settingsManager.GetValue("VirtoCommerce.ModulesAutoInstalled", false))
            {
                lock (_lockObject)
                {
                    if (!_settingsManager.GetValue("VirtoCommerce.ModulesAutoInstalled", false))
                    {
                        var moduleBundles = _extModuleOptions.AutoInstallModuleBundles;
                        if (!moduleBundles.IsNullOrEmpty())
                        {
                            _settingsManager.SetValue("VirtoCommerce.ModulesAutoInstalled", true);

                            EnsureModulesCatalogInitialized();

                            var modules = new List<ManifestModuleInfo>();
                            var moduleVersionGroups = _moduleCatalog.Modules
                                .OfType<ManifestModuleInfo>()
                                .Where(x => x.Groups.Intersect(moduleBundles, StringComparer.OrdinalIgnoreCase).Any())
                                .GroupBy(x => x.Id);

                            //Need install only latest versions
                            foreach (var moduleVersionGroup in moduleVersionGroups)
                            {
                                var alreadyInstalledModule = _moduleCatalog.Modules.OfType<ManifestModuleInfo>().FirstOrDefault(x => x.IsInstalled && x.Id.EqualsInvariant(moduleVersionGroup.Key));
                                //skip already installed modules
                                if (alreadyInstalledModule == null)
                                {
                                    var latestVersion = moduleVersionGroup.OrderBy(x => x.Version).LastOrDefault();
                                    if (latestVersion != null)
                                    {
                                        modules.Add(latestVersion);
                                    }
                                }
                            }

                            var modulesWithDependencies = _moduleCatalog.CompleteListWithDependencies(modules)
                                .OfType<ManifestModuleInfo>()
                                .Where(x => !x.IsInstalled)
                                .Select(x => AbstractTypeFactory<ModuleDescriptor>.TryCreateInstance().FromModel(x))
                                .ToArray();

                            if (modulesWithDependencies.Any())
                            {
                                var options = new ModuleBackgroundJobOptions
                                {
                                    Action = ModuleAction.Install,
                                    Modules = modulesWithDependencies
                                };
                                //reset finished date
                                notification.Finished = null;

                                BackgroundJob.Enqueue(() => ModuleBackgroundJob(options, notification));
                            }
                        }
                    }
                }
            }
            return Ok(notification);
        }

        public void ModuleBackgroundJob(ModuleBackgroundJobOptions options, ModulePushNotification notification)
        {
            try
            {
                notification.Started = DateTime.UtcNow;
                var moduleInfos = _moduleCatalog.Modules.OfType<ManifestModuleInfo>()
                                     .Where(x => options.Modules.Any(y => y.Identity.Equals(x.Identity)))
                                     .ToArray();
                var reportProgress = new Progress<ProgressMessage>(m =>
                {
                    lock (_lockObject)
                    {
                        notification.ProgressLog.Add(m);
                        _pushNotifier.Send(notification);
                    }
                });

                switch (options.Action)
                {
                    case ModuleAction.Install:
                        _moduleInstaller.Install(moduleInfos, reportProgress);
                        break;
                    case ModuleAction.Uninstall:
                        _moduleInstaller.Uninstall(moduleInfos, reportProgress);
                        break;
                }
            }
            catch (Exception ex)
            {
                notification.ProgressLog.Add(new ProgressMessage
                {
                    Level = ProgressMessageLevel.Error,
                    Message = ex.ToString()
                });
            }
            finally
            {
                notification.Finished = DateTime.UtcNow;
                notification.ProgressLog.Add(new ProgressMessage
                {
                    Level = ProgressMessageLevel.Info,
                    Message = "Installation finished.",
                });
                _pushNotifier.Send(notification);
            }
        }

        private void EnsureModulesCatalogInitialized()
        {
            _moduleCatalog.Initialize();
        }

        private IEnumerable<ManifestModuleInfo> GetDependingModulesRecursive(IEnumerable<ManifestModuleInfo> modules)
        {
            var retVal = new List<ManifestModuleInfo>();
            foreach (var module in modules)
            {
                retVal.Add(module);
                var dependingModules = _moduleCatalog.Modules.OfType<ManifestModuleInfo>()
                                                             .Where(x => x.IsInstalled)
                                                             .Where(x => x.DependsOn.Contains(module.Id, StringComparer.OrdinalIgnoreCase))
                                                             .ToList();
                if (dependingModules.Any())
                {
                    retVal.AddRange(GetDependingModulesRecursive(dependingModules));
                }
            }
            return retVal;
        }

        private ModulePushNotification ScheduleJob(ModuleBackgroundJobOptions options)
        {
            var notification = new ModulePushNotification(_userNameResolver.GetCurrentUserName());

            switch (options.Action)
            {
                case ModuleAction.Install:
                    notification.Title = "Install Module";
                    notification.ProgressLog.Add(new ProgressMessage { Level = ProgressMessageLevel.Info, Message = "Starting installation..." });
                    break;
                case ModuleAction.Uninstall:
                    notification.Title = "Uninstall Module";
                    notification.ProgressLog.Add(new ProgressMessage { Level = ProgressMessageLevel.Info, Message = "Starting uninstall..." });
                    break;
            }

            _pushNotifier.Send(notification);

            BackgroundJob.Enqueue(() => ModuleBackgroundJob(options, notification));

            return notification;
        }
    }
}
