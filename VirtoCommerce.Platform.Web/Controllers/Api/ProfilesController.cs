using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private static object _lockObject = new object();
        private readonly ISettingsManager _settingsManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfilesController(UserManager<ApplicationUser> userManager, ISettingsManager settingsManager)
        {
            _userManager = userManager;
            _settingsManager = settingsManager;
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("currentuser")]
        [Authorize]
        public async Task<ActionResult> GetCurrentUserProfile()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (currentUser != null)
            {
                var userProfile = new UserProfile()
                {
                    Id = currentUser.Id
                };
                var modules = _settingsManager.GetModules();
                userProfile.Settings = modules.SelectMany(module => _settingsManager.GetModuleSettings(module.Id)).Where(setting => setting.GroupName == "Platform|User Profile").ToArray();
                _settingsManager.LoadEntitySettingsValues(userProfile);
                return Ok(userProfile);
            }
            return NotFound();
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("currentuser")]
        [Authorize]
        public async Task<ActionResult> UpdateCurrentUserProfile([FromBody] UserProfile userProfile)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (currentUser.Id != userProfile.Id)
            {
                return Unauthorized();
            }
            lock (_lockObject)
            {
                _settingsManager.SaveEntitySettingsValues(userProfile);
            }
            return Ok();
        }
    }
}
