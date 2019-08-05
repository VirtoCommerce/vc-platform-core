using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.LicensingModule.Core;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.LicensingModule.Core.Services;

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
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<LicenseSearchResult>> SearchLicenses(LicenseSearchCriteria request)
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
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<License>> GetLicenseById(string id)
        {
            var retVal = await _licenseService.GetByIdsAsync(new[] { id });
            return Ok(retVal.FirstOrDefault());
        }

        [HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<License>> CreateLicense([FromBody]License license)
        {
            await _licenseService.SaveChangesAsync(new[] { license });
            return Ok(license);
        }

        [HttpPut]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult<License>> UpdateLicense([FromBody]License license)
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
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<ActionResult> DeleteLicensesByIds([FromQuery] string[] ids)
        {
            await _licenseService.DeleteAsync(ids);
            return Ok();
        }

        [HttpGet]
        [Route("download/{activationCode}")]
        [Authorize(ModuleConstants.Security.Permissions.Issue)]
        public async Task<ContentResult> Download(string activationCode)
        {
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            return await GetSignedLicenseAsync(activationCode, false, remoteIpAddress.ToString());
        }

        [HttpGet]
        [Route("activate/{activationCode}")]
        [AllowAnonymous]
        public async Task<ContentResult> Activate(string activationCode)
        {
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            return await GetSignedLicenseAsync(activationCode, true, remoteIpAddress.ToString());
        }


        private async Task<ContentResult> GetSignedLicenseAsync(string activationCode, bool isActivated, string ip)
        {
            var signedLicense = await _licenseService.GetSignedLicenseAsync(activationCode, ip, isActivated);

            if (!string.IsNullOrEmpty(signedLicense))
            {
                var result = new ContentResult
                {
                    Content = signedLicense,
                    StatusCode = 200,
                    ContentType = "application/octet-stream"
                };

                var contentDisposition = new System.Net.Mime.ContentDisposition("attachment")
                {
                    FileName = "VirtoCommerce.lic"
                };
                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                return result;
            }

            return new ContentResult { StatusCode = 400 };
        }

    }
}
