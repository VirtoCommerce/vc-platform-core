using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Helpers;
using VirtoCommerce.Platform.Web.Extensions;
using VirtoCommerce.Platform.Web.Helpers;
using VirtoCommerce.Platform.Web.Infrastructure;
using VirtoCommerce.Platform.Web.Swagger;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{
    [Route("api/platform/assets")]
    public class AssetsController : Controller
    {
        private readonly string _uploadsUrl;
        private readonly IBlobStorageProvider _blobProvider;
        private readonly IBlobUrlResolver _urlResolver;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public AssetsController(IBlobStorageProvider blobProvider, IBlobUrlResolver urlResolver, IOptions<PlatformOptions> option)
        {
            _blobProvider = blobProvider;
            _urlResolver = urlResolver;

            _uploadsUrl = option?.Value?.UploadUrl;

            if (_uploadsUrl == null)
                throw new PlatformException($"{nameof(option.Value.UploadUrl)} must be set");
        }

        /// <summary>
        /// This method used to upload files on local disk storage in special uploads folder
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("localstorage")]
        [DisableFormValueModelBinding]
        [ProducesResponseType(typeof(BlobInfo[]), 200)]
        [Authorize(SecurityConstants.Permissions.AssetCreate)]
        public async Task<IActionResult> UploadAssetToLocalFileSystemAsync()
        {
            //ToDo Now supports downloading one file, find a solution for downloading multiple files
            var retVal = new List<BlobInfo>();

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            string targetFilePath = null;

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            if (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        //ToDo After update to core 2.1 make beautiful https://github.com/aspnet/HttpAbstractions/issues/446
                        var fileName = contentDisposition.FileName.Value.TrimStart('\"').TrimEnd('\"');

                        var uploadsPath = Path.GetFullPath(_uploadsUrl);

                        CheckPath(uploadsPath);

                        targetFilePath = Path.Combine(uploadsPath, fileName);

                        using (var targetStream = System.IO.File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                        }

                        var blobInfo = AbstractTypeFactory<BlobInfo>.TryCreateInstance();
                        blobInfo.Name = fileName;
                        blobInfo.Url = _uploadsUrl + fileName;
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
        [Authorize(SecurityConstants.Permissions.AssetCreate)]
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
                    ContentDispositionHeaderValue contentDisposition;
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                    if (hasContentDispositionHeader)
                    {
                        if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                        {
                            //ToDo After update to core 2.1 make beautiful https://github.com/aspnet/HttpAbstractions/issues/446
                            var fileName = contentDisposition.FileName.Value.TrimStart('\"').TrimEnd('\"');

                            targetFilePath = folderUrl + "/" + fileName;

                            using (var targetStream = _blobProvider.OpenWrite(targetFilePath))
                            {
                                await section.Body.CopyToAsync(targetStream);
                            }

                            var blobInfo = AbstractTypeFactory<BlobInfo>.TryCreateInstance();
                            blobInfo.Name = fileName;
                            blobInfo.Url = _uploadsUrl + fileName;
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
        [Authorize(SecurityConstants.Permissions.AssetDelete)]
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
        [Authorize(SecurityConstants.Permissions.AssetRead)]
        public async Task<IActionResult> SearchAssetItemsAsync([FromQuery]string folderUrl = null,[FromQuery] string keyword = null)
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
        [Authorize(SecurityConstants.Permissions.AssetCreate)]
        public async Task<IActionResult> CreateBlobFolderAsync([FromBody]BlobFolder folder)
        {
            await _blobProvider.CreateFolderAsync(folder);
            return Ok();
        }

        private void CheckPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
