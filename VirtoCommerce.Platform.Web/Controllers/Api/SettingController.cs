using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/platform/settings")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SettingController : Controller
    {
        private readonly ISettingsManager _settingsManager;
        private readonly ILocalModuleCatalog _moduleCatalog;

        public SettingController(ISettingsManager settingsManager, ILocalModuleCatalog moduleCatalog)
        {
            _settingsManager = settingsManager;
            _moduleCatalog = moduleCatalog;
        }

        /// <summary>
        /// Get all settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(SettingEntry[]), 200)]
        public async Task<IActionResult> GetAllSettingsAsync()
        {
            var modules = _moduleCatalog.Modules.OfType<ManifestModuleInfo>();
            var result = new List<SettingEntry>();
            foreach (var module in modules)
            {
                result.AddRange((await _settingsManager.GetModuleSettingsAsync(module.Id)).Where(x => !x.IsRuntime));
            }
            return Ok(result);
        }

        /// <summary>
        /// Get settings registered by specific module
        /// </summary>
        /// <param name="id">Module ID.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("modules/{id}")]
        [ProducesResponseType(typeof(SettingEntry[]), 200)]
        [Authorize(PlatformConstants.Security.Permissions.SettingQuery)]
        public async Task<IActionResult> GetModuleSettingsAsync(string id)
        {
            var result = await _settingsManager.GetModuleSettingsAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Get setting details by name
        /// </summary>
        /// <param name="name">Setting system name.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{name}")]
        [ProducesResponseType(typeof(SettingEntry), 200)]
        [Authorize(PlatformConstants.Security.Permissions.SettingAccess)]
        public async Task<IActionResult> GetSettingAsync(string name)
        {
            var result = await _settingsManager.GetSettingByNameAsync(name);
            return Ok(result);
        }

        /// <summary>
        /// Update settings values
        /// </summary>
        /// <param name="settings"></param>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(void), 200)]
        [Authorize(PlatformConstants.Security.Permissions.SettingUpdate)]
        public async Task<IActionResult> UpdateAsync([FromBody] SettingEntry[] settings)
        {
            using (await AsyncLock.GetLockByKey("settings").LockAsync())
            {
                await _settingsManager.SaveSettingsAsync(settings);
            }
            return Ok();
        }

        /// <summary>
        /// Get array setting values by name
        /// </summary>
        /// <param name="name">Setting system name.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("values/{name}")]
        [ProducesResponseType(typeof(object[]), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetArrayAsync(string name)
        {
            var result = await _settingsManager.GetArrayAsync<object>(name, null);
            return Ok(result);
        }

        /// <summary>
        /// Get UI customization setting
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("ui/customization")]
        [ProducesResponseType(typeof(SettingEntry), 200)]
        public async Task<IActionResult> GetUICustomizationSetting()
        {
            var result = await _settingsManager.GetSettingByNameAsync(PlatformConstants.Settings.UserInterface.Customization.Name);
            return Ok(result);
        }
    }
}
