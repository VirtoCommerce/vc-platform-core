using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Web.Model.Profiles;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/platform/profiles")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ProfilesController : Controller
    {
        private readonly ISettingsManager _settingsManager;
        private readonly ILocalModuleCatalog _moduleCatalog;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfilesController(UserManager<ApplicationUser> userManager, ISettingsManager settingsManager, ILocalModuleCatalog moduleCatalog)
        {
            _userManager = userManager;
            _settingsManager = settingsManager;
            _moduleCatalog = moduleCatalog;
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("currentuser")]
        [Authorize]
        public async Task<ActionResult> GetCurrentUserProfileAsync()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (currentUser != null)
            {
                var userProfile = AbstractTypeFactory<UserProfile>.TryCreateInstance();
                userProfile.Id = currentUser.Id;
                foreach (var module in _moduleCatalog.Modules.OfType<ManifestModuleInfo>())
                {
                    //TODO: move Platform|User Profile to constants
                    var moduleSetings = (await _settingsManager.GetModuleSettingsAsync(module.Id))
                                            .Where(setting => setting.GroupName == "Platform|User Profile");
                    userProfile.Settings = userProfile.Settings.Concat(moduleSetings).Distinct().ToList();
                }
                await _settingsManager.LoadEntitySettingsValuesAsync(userProfile);
                return Ok(userProfile);
            }
            return Ok();
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("currentuser")]
        [Authorize]
        public async Task<ActionResult> UpdateCurrentUserProfileAsync([FromBody] UserProfile userProfile)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (currentUser.Id != userProfile.Id)
            {
                return Unauthorized();
            }
            using (await AsyncLock.GetLockByKey(userProfile.ToString()).LockAsync())
            {
                await _settingsManager.SaveEntitySettingsValuesAsync(userProfile);
            }
            return Ok();
        }
    }
}
