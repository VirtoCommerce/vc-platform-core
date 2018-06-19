using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Helpers;
using VirtoCommerce.Platform.Web.Helpers;
using VirtoCommerce.Platform.Web.Infrastructure;
using VirtoCommerce.Platform.Web.Swagger;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{
    [Route("api/platform/assets")]
    public class AssetsController : Controller
    {
        private readonly IBlobStorageProvider _blobProvider;
        private readonly IBlobUrlResolver _urlResolver;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        private readonly PlatformOptions _platformOptions;

        public AssetsController(IBlobStorageProvider blobProvider, IBlobUrlResolver urlResolver, IOptions<PlatformOptions> platformOptions)
        {
            _blobProvider = blobProvider;
            _urlResolver = urlResolver;
            _platformOptions = platformOptions.Value;
        }

        /// <summary>
        /// This method used to upload files on local disk storage in special uploads folder
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("localstorage")]
        [DisableFormValueModelBinding]
        [ProducesResponseType(typeof(BlobInfo[]), 200)]
        [Authorize(PlatformConstants.Security.Permissions.AssetCreate)]
        public async Task<IActionResult> UploadAssetToLocalFileSystemAsync()
        {
            //ToDo Now supports downloading one file, find a solution for downloading multiple files
            var retVal = new List<BlobInfo>();

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            var uploadPath = Path.GetFullPath(_platformOptions.LocalUploadFolderPath);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            string targetFilePath = null;

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            if (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out ContentDispositionHeaderValue contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        var fileName = contentDisposition.FileName.Value;
                        targetFilePath = Path.Combine(uploadPath, fileName);

                        using (var targetStream = System.IO.File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                        }

                        var blobInfo = AbstractTypeFactory<BlobInfo>.TryCreateInstance();
                        blobInfo.Name = fileName;
                        //Use only file name as Url, for further access to these files need use PlatformOptions.LocalUploadFolderPath 
                        blobInfo.Url = fileName;
                        blobInfo.ContentType = MimeTypeResolver.ResolveContentType(fileName);
                        retVal.Add(blobInfo);
                    }

                }
            }
            return Ok(retVal.ToArray());
        }

        /// <summary>
        /// Upload assets to the folder
        /// </summary>
        /// <remarks>
        /// Request body can contain multiple files.
        /// </remarks>
        /// <param name="folderUrl">Parent folder url (relative or absolute).</param>
        /// <param name="url">Url for uploaded remote resource (optional)</param>
        /// <param name="name">File name</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [DisableFormValueModelBinding]
        [ProducesResponseType(typeof(BlobInfo[]), 200)]
        [Authorize(PlatformConstants.Security.Permissions.AssetCreate)]
        [UploadFile]
        public async Task<IActionResult> UploadAssetAsync([FromQuery] string folderUrl, [FromQuery]string url = null, [FromQuery]string name = null)
        {
            if (url == null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var retVal = new List<BlobInfo>();
            if (url != null)
            {
                var fileName = name ?? HttpUtility.UrlDecode(Path.GetFileName(url));
                var fileUrl = folderUrl + "/" + fileName;
                using (var client = new WebClient())
                using (var blobStream = _blobProvider.OpenWrite(fileUrl))
                using (var remoteStream = client.OpenRead(url))
                {
                    remoteStream.CopyTo(blobStream);
                    var blobInfo = AbstractTypeFactory<BlobInfo>.TryCreateInstance();
                    blobInfo.Name = fileName;
                    blobInfo.RelativeUrl = fileUrl;
                    blobInfo.Url = _urlResolver.GetAbsoluteUrl(fileUrl);
                    retVal.Add(blobInfo);
                }
            }
            else
            {
                string targetFilePath = null;

                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
                var reader = new MultipartReader(boundary, HttpContext.Request.Body);

                var section = await reader.ReadNextSectionAsync();
                if (section != null)
                {
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out ContentDispositionHeaderValue contentDisposition);

                    if (hasContentDispositionHeader)
                    {
                        if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                        {
                            var fileName = contentDisposition.FileName.Value;

                            targetFilePath = folderUrl + "/" + fileName;

                            using (var targetStream = _blobProvider.OpenWrite(targetFilePath))
                            {
                                await section.Body.CopyToAsync(targetStream);
                            }

                            var blobInfo = AbstractTypeFactory<BlobInfo>.TryCreateInstance();
                            blobInfo.Name = fileName;
                            blobInfo.RelativeUrl = targetFilePath;
                            blobInfo.Url = _urlResolver.GetAbsoluteUrl(targetFilePath);
                            blobInfo.ContentType = MimeTypeResolver.ResolveContentType(fileName);
                            retVal.Add(blobInfo);
                        }

                    }
                }
            }

            return Ok(retVal.ToArray());
        }

        /// <summary>
        /// Delete blobs by urls
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(void), 200)]
        [Route("")]
        [Authorize(PlatformConstants.Security.Permissions.AssetDelete)]
        public async Task<IActionResult> DeleteBlobsAsync([FromQuery] string[] urls)
        {
            await _blobProvider.RemoveAsync(urls);
            return Ok();
        }

        /// <summary>
        /// SearchAsync asset folders and blobs
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <param name="keyword"></param>
        /// <returns></returns> 
        [HttpGet]
        [ProducesResponseType(typeof(GenericSearchResult<BlobEntry>), 200)]
        [Route("")]
        [Authorize(PlatformConstants.Security.Permissions.AssetRead)]
        public async Task<IActionResult> SearchAssetItemsAsync([FromQuery]string folderUrl = null, [FromQuery] string keyword = null)
        {
            var result = await _blobProvider.SearchAsync(folderUrl, keyword);
            return Ok(result);
        }

        /// <summary>
        /// Create new blob folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(void), 200)]
        [Route("folder")]
        [Authorize(PlatformConstants.Security.Permissions.AssetCreate)]
        public async Task<IActionResult> CreateBlobFolderAsync([FromBody]BlobFolder folder)
        {
            await _blobProvider.CreateFolderAsync(folder);
            return Ok();
        }
    }
}
