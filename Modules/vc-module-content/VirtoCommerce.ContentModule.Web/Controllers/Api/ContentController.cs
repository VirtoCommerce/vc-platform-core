using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Extensions;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.ContentModule.Web.Filters;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Helpers;
using Permissions = VirtoCommerce.ContentModule.Core.ContentConstants.Security.Permissions;

namespace VirtoCommerce.ContentModule.Web.Controllers.Api
{
    [Route("api/content/{contentType}/{storeId}")]
    public class ContentController : Controller
    {
        private readonly IBlobContentStorageProviderFactory _blobContentStorageProviderFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public ContentController(
            IBlobContentStorageProviderFactory blobContentStorageProviderFactory
            , IPlatformMemoryCache platformMemoryCache)
        {
            _blobContentStorageProviderFactory = blobContentStorageProviderFactory;
            _platformMemoryCache = platformMemoryCache;
        }

        /// <summary>
        /// Return summary content statistic 
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns>Object contains counters with main content types</returns>
        [HttpGet]
        [Route("~/api/content/{storeId}/stats")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<ContentStatistic>> GetStoreContentStatsAsync(string storeId)
        {
            var contentStorageProvider = _blobContentStorageProviderFactory.CreateProvider("");
            var cacheKey = CacheKey.With(GetType(), "pagesCount", $"content-{storeId}");
            var pagesCount = _platformMemoryCache.GetOrCreateExclusive(cacheKey, cacheEntry =>
            {
                cacheEntry.AddExpirationToken(ContentCacheRegion.CreateChangeToken($"content-{storeId}"));
                var result = CountContentItemsRecursive(GetContentBasePath("pages", storeId), contentStorageProvider, GetContentBasePath("blogs", storeId));
                return result;
            });
            var themesTask = contentStorageProvider.SearchAsync(GetContentBasePath("themes", storeId), null);
            var blogsTask = contentStorageProvider.SearchAsync(GetContentBasePath("blogs", storeId), null);

            await Task.WhenAll(themesTask, blogsTask);

            var themes = themesTask.Result;
            var blogs = blogsTask.Result;

            var retVal = new ContentStatistic
            {
                ActiveThemeName = "default",
                ThemesCount = themes.Results.OfType<BlobFolder>().Count(),
                BlogsCount = blogs.Results.OfType<BlobFolder>().Count(),
                PagesCount = pagesCount
            };

            return Ok(retVal);
        }


        /// <summary>
        /// Delete content from server
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id </param>
        /// <param name="urls">relative content urls to delete</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Authorize(Permissions.Delete)]
        public async Task<ActionResult> DeleteContentAsync(string contentType, string storeId, [FromQuery] string[] urls)
        {
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));
            await storageProvider.RemoveAsync(urls);

            //ToDo Reset cached items
            //_cacheManager.ClearRegion($"content-{storeId}");
            ContentCacheRegion.ExpireRegion();
            return NoContent();
        }

        /// <summary>
        /// Return streamed data for requested by relativeUrl content (Used to prevent Cross domain requests in manager)
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="relativeUrl">content relative url</param>
        /// <returns>stream</returns>
        [HttpGet]
        [Route("")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<byte[]>> GetContentItemDataStream(string contentType, string storeId, [FromQuery] string relativeUrl)
        {
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));
            if ((await storageProvider.GetBlobInfoAsync(relativeUrl)) != null)
            {
                var fileStream = storageProvider.OpenRead(relativeUrl);
                return File(fileStream, MimeTypeResolver.ResolveContentType(relativeUrl));
            }
            return NotFound();
        }

        /// <summary>
        /// Search content items in specified folder and using search keyword
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">relative path for folder where content items will be searched</param>
        /// <param name="keyword">search keyword</param>
        /// <returns>content items</returns>
        [HttpGet]
        [Route("search")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<ContentItem[]>> SearchContentAsync(string contentType, string storeId, [FromQuery] string folderUrl = null, [FromQuery] string keyword = null)
        {
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));

            var result = await storageProvider.SearchAsync(folderUrl, keyword);
            var retVal = result.Results.OfType<BlobFolder>()
                                .Select(x => x.ToContentModel())
                                .OfType<ContentItem>()
                                .Concat(result.Results.OfType<BlobInfo>().Select(x => x.ToContentModel()))
                                .ToArray();
            return Ok(retVal);
        }

        /// <summary>
        /// Rename or move content item
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="oldUrl">old content item relative or absolute url</param>
        /// <param name="newUrl">new content item relative or absolute url</param>
        /// <returns></returns>
        [HttpGet]
        [Route("move")]
        [Authorize(Permissions.Update)]
        public ActionResult MoveContent(string contentType, string storeId, string oldUrl, string newUrl)
        {
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));

            storageProvider.Move(oldUrl, newUrl);
            return NoContent();
        }

        /// <summary>
        /// Copy contents
        /// </summary>
        /// <param name="srcPath">source content  relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/content/copy")]
        [Authorize(Permissions.Update)]
        public ActionResult CopyContent(string srcPath, string destPath)
        {
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(string.Empty);

            //This method used only for default themes copying that we use string.Empty instead storeId because default themes placed only in root content folder
            storageProvider.Copy(srcPath, destPath);
            return NoContent();
        }

        /// <summary>
        /// Unpack contents
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="archivePath">archive file relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns></returns>
        [HttpGet]
        [Route("unpack")]
        [Authorize(Permissions.Update)]
        public async Task<ActionResult> UnpackAsync(string contentType, string storeId, string archivePath, string destPath)
        {
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));

            using (var stream = storageProvider.OpenRead(archivePath))
            using (var archive = new ZipArchive(stream))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.FullName.EndsWith("/"))
                    {
                        var fileName = string.Join("/", entry.FullName.Split('/').Skip(1));
                        using (var entryStream = entry.Open())
                        using (var targetStream = storageProvider.OpenWrite(destPath + "/" + fileName))
                        {
                            entryStream.CopyTo(targetStream);
                        }
                    }
                }
            }
            //remove archive after unpack
            await storageProvider.RemoveAsync(new[] { archivePath });

            return NoContent();
        }

        /// <summary>
        /// Create content folder 
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folder">content folder</param>
        /// <returns></returns>
        [HttpPost]
        [Route("folder")]
        [Authorize(Permissions.Create)]
        public async Task<ActionResult> CreateContentFolderAsync(string contentType, string storeId, [FromBody] ContentFolder folder)
        {
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));

            await storageProvider.CreateFolderAsync(folder.ToBlobModel(AbstractTypeFactory<BlobFolder>.TryCreateInstance()));

            return NoContent();
        }


        /// <summary>
        /// Upload content item 
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">folder relative url where content will be uploaded</param>
        /// <param name="url">external url which will be used to download content item data</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [DisableFormValueModelBinding]
        [Authorize(Permissions.Create)]
        public async Task<ActionResult<ContentItem[]>> UploadContent(string contentType, string storeId, [FromQuery] string folderUrl, [FromQuery]string url = null)
        {
            if (url == null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var retVal = new List<ContentFile>();
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));
            if (url != null)
            {
                var fileName = HttpUtility.UrlDecode(Path.GetFileName(url));
                var fileUrl = folderUrl + "/" + fileName;

                using (var client = new WebClient())
                using (var blobStream = storageProvider.OpenWrite(fileUrl))
                using (var remoteStream = client.OpenRead(url))
                {
                    remoteStream.CopyTo(blobStream);

                    var сontentFile = AbstractTypeFactory<ContentFile>.TryCreateInstance();

                    сontentFile.Name = fileName;
                    сontentFile.Url = storageProvider.GetAbsoluteUrl(fileUrl);
                    retVal.Add(сontentFile);
                }
            }

            else
            {
                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
                var reader = new MultipartReader(boundary, HttpContext.Request.Body);

                var section = await reader.ReadNextSectionAsync();
                if (section != null)
                {
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                    if (hasContentDispositionHeader)
                    {
                        var fileName = contentDisposition.FileName.Value ?? contentDisposition.Name.Value.Replace("\"", string.Empty);

                        var targetFilePath = folderUrl + "/" + fileName;

                        using (var targetStream = storageProvider.OpenWrite(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                        }

                        var contentFile = AbstractTypeFactory<ContentFile>.TryCreateInstance();
                        contentFile.Name = fileName;
                        contentFile.Url = storageProvider.GetAbsoluteUrl(targetFilePath);
                        retVal.Add(contentFile);
                    }
                }
            }

            ContentCacheRegion.ExpireContent(($"content-{storeId}"));

            return Ok(retVal.ToArray());
        }


        private string GetContentBasePath(string contentType, string storeId)
        {
            var retVal = string.Empty;
            if (contentType.EqualsInvariant("themes"))
            {
                retVal = "Themes/" + storeId;
            }
            else if (contentType.EqualsInvariant("pages"))
            {
                retVal = "Pages/" + storeId;
            }
            else if (contentType.EqualsInvariant("blogs"))
            {
                retVal = "Pages/" + storeId + "/blogs";
            }
            return retVal;
        }

        private int CountContentItemsRecursive(string folderUrl, IBlobStorageProvider blobContentStorageProvider, string excludedFolderUrl = null)
        {
            var searchResult = blobContentStorageProvider.SearchAsync(folderUrl, null).GetAwaiter().GetResult();
            var retVal = searchResult.TotalCount
                        + searchResult.Results.OfType<BlobFolder>()
                            .Where(x => excludedFolderUrl == null || !x.RelativeUrl.EndsWith(excludedFolderUrl, StringComparison.InvariantCultureIgnoreCase))
                            .Select(x => CountContentItemsRecursive(x.RelativeUrl, blobContentStorageProvider))
                            .Sum();

            return retVal;
        }
    }
}
