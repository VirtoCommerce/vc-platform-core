using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Data.ExportImport
{
    public class PlatformExportImportManager : IPlatformExportImportManager
    {
        private const string _manifestZipEntryName = "Manifest.json";
        private const string _platformZipEntryName = "PlatformEntries.json";

        private readonly ILocalModuleCatalog _moduleCatalog;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IDynamicPropertyService _dynamicPropertyService;
        private readonly IDynamicPropertySearchService _dynamicPropertySearchService;
        private readonly IPlatformMemoryCache _memoryCache;
        private readonly IPermissionsRegistrar _permissionsProvider;

        public PlatformExportImportManager(UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager, IPermissionsRegistrar permissionsProvider, ISettingsManager settingsManager,
                IDynamicPropertyService dynamicPropertyService, IDynamicPropertySearchService dynamicPropertySearchService, ILocalModuleCatalog moduleCatalog, IPlatformMemoryCache memoryCache)
        {
            _dynamicPropertyService = dynamicPropertyService;
            _userManager = userManager;
            _roleManager = roleManager;
            _settingsManager = settingsManager;
            _moduleCatalog = moduleCatalog;
            _memoryCache = memoryCache;
            _permissionsProvider = permissionsProvider;
            _dynamicPropertySearchService = dynamicPropertySearchService;
        }

        #region IPlatformExportImportManager Members

        public PlatformExportManifest GetNewExportManifest(string author)
        {
            var retVal = new PlatformExportManifest
            {
                Author = author,
                PlatformVersion = PlatformVersion.CurrentVersion.ToString(),
                Modules = InnerGetModulesWithInterface(typeof(IImportSupport)).Select(x => new ExportModuleInfo
                {
                    Id = x.Id,
                    Version = x.Version.ToString(),
                }).ToArray()
            };

            return retVal;
        }

        public PlatformExportManifest ReadExportManifest(Stream stream)
        {
            PlatformExportManifest retVal = null;
            using (var package = new ZipArchive(stream))
            {
                var manifestPart = package.GetEntry(_manifestZipEntryName.ToString());
                using (var manifestStream = manifestPart.Open())
                {
                    retVal = manifestStream.DeserializeJson<PlatformExportManifest>(GetJsonSerializer());
                }
            }
            return retVal;
        }

        public async Task ExportAsync(Stream outStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }

            using (var zipArchive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
            {
                //Export all selected platform entries
                await ExportPlatformEntriesInternalAsync(zipArchive, manifest, progressCallback, cancellationToken);
                //Export all selected  modules
                await ExportModulesInternalAsync(zipArchive, manifest, progressCallback, cancellationToken);

                //Write system information about exported modules
                var manifestZipEntry = zipArchive.CreateEntry(_manifestZipEntryName, CompressionLevel.Optimal);

                //After all modules exported need write export manifest part
                using (var stream = manifestZipEntry.Open())
                {
                    manifest.SerializeJson(stream, GetJsonSerializer());
                }
            }
        }

        public async Task ImportAsync(Stream stream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }

            var progressInfo = new ExportImportProgressInfo();
            progressInfo.Description = "Starting platform import...";
            progressCallback(progressInfo);

            using (var zipArchive = new ZipArchive(stream))
            {
                //Import selected platform entries
                await ImportPlatformEntriesInternalAsync(zipArchive, manifest, progressCallback, cancellationToken);
                //Import selected modules
                await ImportModulesInternalAsync(zipArchive, manifest, progressCallback, cancellationToken);
            }
        }

        #endregion

        private async Task ImportPlatformEntriesInternalAsync(ZipArchive zipArchive, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            var progressInfo = new ExportImportProgressInfo();

            var platformZipEntries = zipArchive.GetEntry(_platformZipEntryName);
            if (platformZipEntries != null)
            {
                PlatformExportEntries platformEntries;
                using (var stream = platformZipEntries.Open())
                {
                    platformEntries = stream.DeserializeJson<PlatformExportEntries>(GetJsonSerializer());
                }

                //Import security objects
                if (manifest.HandleSecurity)
                {
                    progressInfo.Description = $"Import { platformEntries.Users.Count()} users with roles...";
                    progressCallback(progressInfo);

                    foreach (var role in platformEntries.Roles)
                    {
                        //TODO: Test with new and already exist
                        await _roleManager.UpdateAsync(role);
                        foreach (var permission in role.Permissions)
                        {
                            //TODO: Test with new and already exist
                            await _roleManager.AddClaimAsync(role, new Claim(PlatformConstants.Security.Claims.PermissionClaimType, permission.Name));
                        }
                    }

                    //Next create or update users
                    foreach (var user in platformEntries.Users)
                    {
                        if (_userManager.FindByIdAsync(user.Id).Result != null)
                        {
                            await _userManager.UpdateAsync(user);
                        }
                        else
                        {
                            await _userManager.CreateAsync(user);
                        }
                    }
                }

                //Import modules settings
                if (manifest.HandleSettings)
                {
                    //Import dynamic properties
                    await _dynamicPropertyService.SaveDynamicPropertiesAsync(platformEntries.DynamicProperties.ToArray());
                    await _dynamicPropertyService.SaveDictionaryItemsAsync(platformEntries.DynamicPropertyDictionaryItems.ToArray());

                    foreach (var module in manifest.Modules)
                    {
                        await _settingsManager.SaveObjectSettingsAsync(platformEntries.Settings.Where(x => x.ModuleId == module.Id).ToArray());
                    }
                }
            }
        }

        private async Task ExportPlatformEntriesInternalAsync(ZipArchive zipArchive, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            var progressInfo = new ExportImportProgressInfo();
            var platformExportObj = new PlatformExportEntries();

            if (manifest.HandleSecurity)
            {
                //Roles
                platformExportObj.Roles = _roleManager.Roles.ToList();
                if (_roleManager.SupportsRoleClaims)
                {
                    var permissions = _permissionsProvider.GetAllPermissions();
                    foreach (var role in platformExportObj.Roles)
                    {
                        role.Permissions = (await _roleManager.GetClaimsAsync(role)).Join(permissions, c => c.Value, p => p.Name, (c, p) => p).ToArray();
                    }
                }
                //users 
                var usersResult = _userManager.Users.ToArray();
                progressInfo.Description = $"Security: {usersResult.Count()} users exporting...";
                progressCallback(progressInfo);

                foreach (var user in usersResult)
                {
                    var userExt = await _userManager.FindByIdAsync(user.Id);
                    if (userExt != null)
                    {
                        platformExportObj.Users.Add(userExt);
                    }
                }
            }

            //Export setting for selected modules
            if (manifest.HandleSettings)
            {
                progressInfo.Description = "Settings: selected modules settings exporting...";
                progressCallback(progressInfo);
                foreach (var module in manifest.Modules)
                {
                    var moduleSettings = await _settingsManager.GetObjectSettingsAsync(_settingsManager.AllRegisteredSettings.Where(x => x.ModuleId == module.Id).Select(x => x.Name));
                    platformExportObj.Settings = platformExportObj.Settings.Concat(moduleSettings).ToList();
                }
            }

            //Dynamic properties
            progressInfo.Description = "Dynamic properties: load properties...";
            progressCallback(progressInfo);

            platformExportObj.DynamicProperties = (await _dynamicPropertySearchService.SearchDynamicPropertiesAsync(new DynamicPropertySearchCriteria { Take = int.MaxValue })).Results;
            platformExportObj.DynamicPropertyDictionaryItems = (await _dynamicPropertySearchService.SearchDictionaryItemsAsync(new DynamicPropertyDictionaryItemSearchCriteria { Take = int.MaxValue })).Results;


            //Create part for platform entries
            var platformEntiriesPart = zipArchive.CreateEntry(_platformZipEntryName, CompressionLevel.Optimal);
            using (var partStream = platformEntiriesPart.Open())
            {
                platformExportObj.SerializeJson(partStream);
            }
        }

        private async Task ImportModulesInternalAsync(ZipArchive zipArchive, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            var progressInfo = new ExportImportProgressInfo();
            foreach (var moduleInfo in manifest.Modules)
            {
                var moduleDescriptor = InnerGetModulesWithInterface(typeof(IImportSupport)).FirstOrDefault(x => x.Id == moduleInfo.Id);
                if (moduleDescriptor != null)
                {
                    var modulePart = zipArchive.GetEntry(moduleInfo.PartUri.TrimStart('/'));
                    using (var modulePartStream = modulePart.Open())
                    {
                        void modulePorgressCallback(ExportImportProgressInfo x)
                        {
                            progressInfo.Description = $"{moduleInfo.Id}: {x.Description}";
                            progressCallback(progressInfo);
                        }
                        if (moduleDescriptor.ModuleInstance is IImportSupport importer)
                        {
                            try
                            {
                                //TODO: Add JsonConverter which will be materialized concrete ExportImport option type 
                                var options = manifest.Options.FirstOrDefault(x => x.ModuleIdentity.Id == moduleDescriptor.Identity.Id);
                                await importer.ImportAsync(modulePartStream, options, modulePorgressCallback, cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                progressInfo.Errors.Add($"{moduleInfo.Id}: {ex.ToString()}");
                                progressCallback(progressInfo);
                            }
                        }
                    }
                }
            }
        }

        private async Task ExportModulesInternalAsync(ZipArchive zipArchive, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            var progressInfo = new ExportImportProgressInfo();

            foreach (var module in manifest.Modules)
            {
                var moduleDescriptor = InnerGetModulesWithInterface(typeof(IImportSupport)).FirstOrDefault(x => x.Id == module.Id);
                if (moduleDescriptor != null)
                {
                    //Create part for module
                    var moduleZipEntryName = module.Id + ".json";
                    var zipEntry = zipArchive.CreateEntry(moduleZipEntryName, CompressionLevel.Optimal);

                    void modulePorgressCallback(ExportImportProgressInfo x)
                    {
                        progressInfo.Description = $"{ module.Id }: { x.Description }";
                        progressCallback(progressInfo);
                    }

                    progressInfo.Description = $"{module.Id}: exporting...";
                    progressCallback(progressInfo);
                    if (moduleDescriptor.ModuleInstance is IExportSupport exporter)
                    {
                        try
                        {
                            //TODO: Add JsonConverter which will be materialized concrete ExportImport option type 
                            var options = manifest.Options.FirstOrDefault(x => x.ModuleIdentity.Id == moduleDescriptor.Identity.Id);
                            await exporter.ExportAsync(zipEntry.Open(), options, modulePorgressCallback, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            progressInfo.Errors.Add($"{ module.Id}: {ex.ToString()}");
                            progressCallback(progressInfo);
                        }
                    }
                    module.PartUri = moduleZipEntryName.ToString();
                }
            }
        }

        private ManifestModuleInfo[] InnerGetModulesWithInterface(Type interfaceType)
        {
            var retVal = _moduleCatalog.Modules.OfType<ManifestModuleInfo>().Where(x => x.ModuleInstance != null)
                                        .Where(x => x.ModuleInstance.GetType().GetInterfaces().Contains(interfaceType))
                                        .ToArray();
            return retVal;
        }

        private static JsonSerializer GetJsonSerializer()
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return serializer;
        }

    }
}
