using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{

    [Produces("application/json")]
    [Route("api/platform/settings")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SettingController : Controller
    {
        private readonly ISettingsManager _settingsManager;
        private static object _lock = new object();

        public SettingController(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        /// <summary>
        /// Get all settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(SettingEntry[]), 200)]
        public IActionResult GetAllSettings()
        {
            var modules = _settingsManager.GetModules();
            return Ok(modules.SelectMany(x => _settingsManager.GetModuleSettings(x.Id)).Where(x => !x.IsRuntime).ToArray());
        }

        /// <summary>
        /// Get settings registered by specific module
        /// </summary>
        /// <param name="id">Module ID.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("modules/{id}")]
        [ProducesResponseType(typeof(SettingEntry[]), 200)]
        public IActionResult GetModuleSettings(string id)
        {
            var retVal = _settingsManager.GetModuleSettings(id);
            return Ok(retVal);
        }

        /// <summary>
        /// Get setting details by name
        /// </summary>
        /// <param name="name">Setting system name.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{name}")]
        [ProducesResponseType(typeof(SettingEntry), 200)]
        public IActionResult GetSetting(string name)
        {
            var retVal = _settingsManager.GetSettingByName(name);
            if (retVal != null)
            {
                return Ok(retVal);
            }
            return NotFound();
        }

        /// <summary>
        /// Update settings values
        /// </summary>
        /// <param name="settings"></param>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(void), 200)]
        //[CheckPermission(Permission = PredefinedPermissions.SettingUpdate)]
        public IActionResult Update([FromBody]SettingEntry[] settings)
        {
            lock (_lock)
            {
                _settingsManager.SaveSettings(settings);
            }

            return StatusCode((int)HttpStatusCode.NoContent);
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
        public IActionResult GetArray(string name)
        {
            var value = _settingsManager.GetArray<object>(name, null);
            return Ok(value);
        }

        /// <summary>
        /// Get UI customization setting
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("ui/customization")]
        [ProducesResponseType(typeof(SettingEntry), 200)]
        public IActionResult GetUICustomizationSetting()
        {
            var retVal = _settingsManager.GetSettingByName("VirtoCommerce.Platform.UI.Customization");
            if (retVal != null)
            {
                return Ok(retVal);
            }
            return NotFound();
        }
    }
}
