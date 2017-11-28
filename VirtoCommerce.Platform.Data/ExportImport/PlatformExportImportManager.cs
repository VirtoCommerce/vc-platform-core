using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Data.ExportImport
{
    public sealed class PlatformExportEntries
    {
        public PlatformExportEntries()
        {
            Users = new List<ApplicationUserExtended>();
            Settings = new List<SettingEntry>();
            DynamicPropertyDictionaryItems = new List<DynamicPropertyDictionaryItem>();
        }
        public bool IsNotEmpty
        {
            get
            {
                return Users.Any() || Settings.Any();
            }
        }
        public ICollection<ApplicationUserExtended> Users { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<SettingEntry> Settings { get; set; }
        public ICollection<DynamicPropertyDictionaryItem> DynamicPropertyDictionaryItems { get; set; }
        public ICollection<DynamicProperty> DynamicProperties { get; set; }

    }



    public class PlatformExportImportManager : IPlatformExportImportManager
    {
        private const string _manifestZipEntryName = "Manifest.json";
        private const string _platformZipEntryName = "PlatformEntries.json";

        private readonly IModuleCatalog _moduleCatalog;
        private readonly ISecurityService _securityService;
        private readonly IRoleManagementService _roleManagementService;
        private readonly ISettingsManager _settingsManager;
        private readonly IDynamicPropertyService _dynamicPropertyService;
        private readonly IMemoryCache _memoryCache;

        public PlatformExportImportManager(ISecurityService securityService, IRoleManagementService roleManagementService, ISettingsManager settingsManager, IDynamicPropertyService dynamicPropertyService, IModuleCatalog moduleCatalog, IMemoryCache memoryCache)
        {
            _dynamicPropertyService = dynamicPropertyService;
            _securityService = securityService;
            _roleManagementService = roleManagementService;
            _settingsManager = settingsManager;
            _moduleCatalog = moduleCatalog;
            _memoryCache = memoryCache;       
        }

        #region IPlatformExportImportManager Members

        public PlatformExportManifest GetNewExportManifest(string author)
        {
            var retVal = new PlatformExportManifest
            {
                Author = author,
                PlatformVersion = PlatformVersion.CurrentVersion.ToString(),
                Modules = InnerGetModulesWithInterface(typeof(ISupportExportImportModule)).Select(x => new ExportModuleInfo
                {
                    Id = x.Id,
                    Version = x.Version.ToString(),
                    Description = ((ISupportExportImportModule)x.ModuleInstance).ExportDescription
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

        public void Export(Stream outStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException("manifest");
            }

            using (var zipArchive = new ZipArchive(outStream))
            {
                //Export all selected platform entries
                ExportPlatformEntriesInternal(zipArchive, manifest, progressCallback);
                //Export all selected  modules
                ExportModulesInternal(zipArchive, manifest, progressCallback);

                //Write system information about exported modules
                var manifestZipEntry = zipArchive.CreateEntry(_manifestZipEntryName, CompressionLevel.Optimal);

                //After all modules exported need write export manifest part
                using (var stream = manifestZipEntry.Open())
                {
                    manifest.SerializeJson(stream, GetJsonSerializer());
                }
            }
        }

        public void Import(Stream stream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException("manifest");
            }

            var progressInfo = new ExportImportProgressInfo();
            progressInfo.Description = "Starting platform import...";
            progressCallback(progressInfo);

            using (var zipArchive = new ZipArchive(stream))
            {
                //Import selected platform entries
                ImportPlatformEntriesInternal(zipArchive, manifest, progressCallback);
                //Import selected modules
                ImportModulesInternal(zipArchive, manifest, progressCallback);
                //Reset cache
                //TODO:
                //_memoryCache.Clear();

            }
        }

        #endregion

        private void ImportPlatformEntriesInternal(ZipArchive zipArchive, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
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

                    //First need import roles
                    foreach (var role in platformEntries.Roles)
                    {
                        _roleManagementService.AddOrUpdateRole(role);
                    }
                    //Next create or update users
                    foreach (var user in platformEntries.Users)
                    {
                        if (_securityService.FindByIdAsync(user.Id, UserDetails.Reduced).Result != null)
                        {
                            var dummy = _securityService.UpdateAsync(user).Result;
                        }
                        else
                        {
                            var dummy = _securityService.CreateAsync(user).Result;
                        }
                    }
                }

                //Import modules settings
                if (manifest.HandleSettings)
                {
                    //Import dynamic properties
                    _dynamicPropertyService.SaveProperties(platformEntries.DynamicProperties.ToArray());
                    foreach (var propDicGroup in platformEntries.DynamicPropertyDictionaryItems.GroupBy(x => x.PropertyId))
                    {
                        _dynamicPropertyService.SaveDictionaryItems(propDicGroup.Key, propDicGroup.ToArray());
                    }

                    foreach (var module in manifest.Modules)
                    {
                        _settingsManager.SaveSettings(platformEntries.Settings.Where(x => x.ModuleId == module.Id).ToArray());
                    }
                }           
            }
        }

        private void ExportPlatformEntriesInternal(ZipArchive zipArchive, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var progressInfo = new ExportImportProgressInfo();
            var platformExportObj = new PlatformExportEntries();

            if (manifest.HandleSecurity)
            {
                //Roles
                platformExportObj.Roles = _roleManagementService.SearchRoles(new RoleSearchRequest { SkipCount = 0, TakeCount = int.MaxValue }).Roles;
                //users 
                var usersResult = Task.Run(() => _securityService.SearchUsersAsync(new UserSearchRequest { TakeCount = int.MaxValue })).Result;
                progressInfo.Description = $"Security: {usersResult.Users.Count()} users exporting...";
                progressCallback(progressInfo);

                foreach (var user in usersResult.Users)
                {
                    var userExt = Task.Run(() => _securityService.FindByIdAsync(user.Id, UserDetails.Export)).Result;
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

                platformExportObj.Settings = manifest.Modules.SelectMany(x => _settingsManager.GetModuleSettings(x.Id)).ToList();
            }

            //Dynamic properties
            var allTypes = _dynamicPropertyService.GetAvailableObjectTypeNames();

            progressInfo.Description = "Dynamic properties: load properties...";
            progressCallback(progressInfo);

            platformExportObj.DynamicProperties = allTypes.SelectMany(x => _dynamicPropertyService.GetProperties(x)).ToList();
            platformExportObj.DynamicPropertyDictionaryItems = platformExportObj.DynamicProperties.Where(x => x.IsDictionary).SelectMany(x => _dynamicPropertyService.GetDictionaryItems(x.Id)).ToList();

            //Notification templates
            progressInfo.Description = "Notifications: load templates...";
            progressCallback(progressInfo);

            //Create part for platform entries
            var platformEntiriesPart = zipArchive.CreateEntry(_platformZipEntryName, CompressionLevel.Optimal);
            using (var partStream = platformEntiriesPart.Open())
            {
                platformExportObj.SerializeJson(partStream);
            }
        }

        private void ImportModulesInternal(ZipArchive zipArchive, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var progressInfo = new ExportImportProgressInfo();
            foreach (var moduleInfo in manifest.Modules)
            {
                var moduleDescriptor = InnerGetModulesWithInterface(typeof(ISupportExportImportModule)).FirstOrDefault(x => x.Id == moduleInfo.Id);
                if (moduleDescriptor != null)
                {
                    var modulePart = zipArchive.GetEntry(moduleInfo.PartUri);
                    using (var modulePartStream = modulePart.Open())
                    {
                        Action<ExportImportProgressInfo> modulePorgressCallback = (x) =>
                        {
                            progressInfo.Description = $"{moduleInfo.Id}: {x.Description}";
                            progressCallback(progressInfo);
                        };
                        try
                        {
                            ((ISupportExportImportModule)moduleDescriptor.ModuleInstance).DoImport(modulePartStream, manifest, modulePorgressCallback);
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

        private void ExportModulesInternal(ZipArchive zipArchive, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var progressInfo = new ExportImportProgressInfo();

            foreach (var module in manifest.Modules)
            {
                var moduleDescriptor = InnerGetModulesWithInterface(typeof(ISupportExportImportModule)).FirstOrDefault(x => x.Id == module.Id);
                if (moduleDescriptor != null)
                {
                    //Create part for module
                    var moduleZipEntryName = module.Id + ".json";
                    var zipEntry = zipArchive.CreateEntry(moduleZipEntryName, CompressionLevel.Optimal);

                    Action<ExportImportProgressInfo> modulePorgressCallback = (x) =>
                    {
                        progressInfo.Description = $"{ module.Id }: { x.Description }";
                        progressCallback(progressInfo);
                    };

                    progressInfo.Description = $"{module.Id}: exporting...";
                    progressCallback(progressInfo);

                    try
                    {
                        ((ISupportExportImportModule)moduleDescriptor.ModuleInstance).DoExport(zipEntry.Open(), manifest, modulePorgressCallback);
                    }
                    catch (Exception ex)
                    {
                        progressInfo.Errors.Add($"{ module.Id}: {ex.ToString()}");
                        progressCallback(progressInfo);
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
