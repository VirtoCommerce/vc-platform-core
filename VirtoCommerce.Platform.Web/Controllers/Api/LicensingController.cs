using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Web.Extensions;
using VirtoCommerce.Platform.Web.Licensing;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/platform/licensing")]
    //[Authorize(SecurityConstants.Permissions.ModuleManage)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LicensingController : Controller
    {
        private readonly IHostingEnvironment _hostingEnv;
        public LicensingController(IHostingEnvironment hostingEnv)
        {
            _hostingEnv = hostingEnv;
        }

        [HttpPost]
        [Route("activateByCode")]
        [ProducesResponseType(typeof(License), 200)]
        public async Task<IActionResult> ActivateByCode([FromBody]string activationCode)
        {
            License license = null;

            using (var httpClient = new HttpClient())
            {
                var activationUrl = new Uri("https://virtocommerce.com/admin/api/licenses/activate/" + activationCode);
                var httpResponse = await httpClient.GetAsync(activationUrl);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var rawLicense = await httpResponse.Content.ReadAsStringAsync();
                    license = License.Parse(rawLicense);
                }
            }

            return Ok(license);
        }

        [HttpPost]
        [Route("activateByFile")]
        [ProducesResponseType(typeof(License), 200)]
        public async Task<IActionResult> ActivateByFile(IFormFile file)
        {
            License license = null;
            var rawLicense = string.Empty;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                rawLicense = reader.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(rawLicense))
            {
                license = License.Parse(rawLicense);
            }
            
            return Ok(license);
        }

        [HttpPost]
        [Route("activateLicense")]
        [ProducesResponseType(typeof(License), 200)]
        public IActionResult ActivateLicense(License license)
        {
            license = License.Parse(license?.RawLicense);

            if (license != null)
            {

                //var licenseFilePath = HostingEnvironment.MapPath(Startup.VirtualRoot + "/App_Data/VirtoCommerce.lic");
                var licenseFilePath = _hostingEnv.MapPath("~/App_Data/VirtoCommerce.lic");
                //File.WriteAllText(licenseFilePath, license.RawLicense);
                File(licenseFilePath, license.RawLicense);
            }

            return Ok(license);
        }
    }
}
