using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.LicensingModule.Core.Security;
using VirtoCommerce.LicensingModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Web.Controllers.Api
{
    [Route("api/licenses")]
    public class LicensingModuleController : Controller
    {
        private readonly ILicenseService _licenseService;
        
        public LicensingModuleController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(GenericSearchResult<License>), 200)]
        [Authorize(PredefinedPermissions.Read)]
        public async Task<IActionResult> SearchLicenses(LicenseSearchCriteria request)
        {
            if (request == null)
            {
                return BadRequest("request is null");
            }

            var searchResponse = await _licenseService.SearchAsync(request);

            return Ok(searchResponse);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(License), 200)]
        [Authorize(PredefinedPermissions.Read)]
        public async Task<IActionResult> GetLicenseById(string id)
        {
            var retVal = await _licenseService.GetByIdsAsync(new[] { id });
            return Ok(retVal.FirstOrDefault());
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(License), 200)]
        [Authorize(PredefinedPermissions.Create)]
        public async Task<IActionResult> CreateLicense([FromBody]License license)
        {
            await _licenseService.SaveChangesAsync(new[] { license });
            return Ok(license);
        }

        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(License), 200)]
        [Authorize(PredefinedPermissions.Update)]
        public async Task<IActionResult> UpdateLicense([FromBody]License license)
        {
            await _licenseService.SaveChangesAsync(new[] { license });
            return Ok(license);
        }

        /// <summary>
        ///  Delete Licenses
        /// </summary>
        /// <param name="ids">Licenses' ids for delete</param>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(typeof(void), 200)]
        [Authorize(PredefinedPermissions.Delete)]
        public async Task<IActionResult> DeleteLicensesByIds([FromQuery] string[] ids)
        {
            await _licenseService.DeleteAsync(ids);
            return Ok();
        }

        [HttpGet]
        [Route("download/{activationCode}")]
        [ProducesResponseType(typeof(ContentResult), 200)]
        [Authorize(PredefinedPermissions.Issue)]
        public Task<IActionResult> Download(string activationCode)
        {
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            return GetSignedLicenseAsync(activationCode, false, remoteIpAddress.ToString());
        }

        [HttpGet]
        [Route("activate/{activationCode}")]
        [ProducesResponseType(typeof(ContentResult), 200)]
        [AllowAnonymous]
        public Task<IActionResult> Activate(string activationCode)
        {
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            return GetSignedLicenseAsync(activationCode, true, remoteIpAddress.ToString());
        }


        private async Task<IActionResult> GetSignedLicenseAsync(string activationCode, bool isActivated, string ip)
        {
            //var clientIp = GetClientIpAddress();
            var signedLicense = await _licenseService.GetSignedLicenseAsync(activationCode, ip, isActivated);

            if (!string.IsNullOrEmpty(signedLicense))
            {
                var result = new ContentResult
                {
                    Content = signedLicense
                    , StatusCode = 200
                    , ContentType = "application/octet-stream"
                };

                var contentDisposition = new System.Net.Mime.ContentDisposition("attachment")
                {
                    FileName = "VirtoCommerce.lic"
                };
                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                return result;
            }

            return new NotFoundResult();
        }

        //private static string GetClientIpAddress(HttpRequestMessage requestMessage)
        //{
        //    var request = (requestMessage.Properties["MS_HttpContext"] as HttpContextWrapper)?.Request;
        //    return request?.ServerVariables["HTTP_X_FORWARDED_FOR"]?.Split(',').FirstOrDefault() ?? request?.ServerVariables["REMOTE_ADDR"] ?? request?.UserHostAddress;
        //}
    }
}
