using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Data.Helpers;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Extension;
using VirtoCommerce.Platform.Web.Helpers;

using Permissions = VirtoCommerce.ContentModule.Core.ContentConstants.Security.Permissions;

namespace VirtoCommerce.ContentModule.Web.Controllers.Api
{
    [Route("api/content/{contentType}/{storeId}")]
    public class ContentController : Controller
    {
        private readonly Func<string, IContentBlobStorageProvider> _contentStorageProviderFactory;
        private readonly IBlobUrlResolver _urlResolver;
        private readonly IStoreService _storeService;
        private readonly IPlatformMemoryCache _memoryCache;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public ContentController(Func<string, IContentBlobStorageProvider> contentStorageProviderFactory, IBlobUrlResolver urlResolver, IStoreService storeService, IPlatformMemoryCache memoryCache)
        {
            _storeService = storeService;
            _contentStorageProviderFactory = contentStorageProviderFactory;
            _urlResolver = urlResolver;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Return summary content statistic 
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns>Object contains counters with main content types</returns>
        [HttpGet]
        [Route("~/api/content/{storeId}/stats")]
        [ProducesResponseType(typeof(ContentStatistic), 200)]
        [Authorize(Permissions.Read)]
        public async Task<IActionResult> GetStoreContentStatsAsync(string storeId)
        {
            var contentStorageProvider = _contentStorageProviderFactory("");

            //var cacheKey = CacheKey.With(GetType(), "GetStoreContentStatsAsync", $"content-{storeId}", TimeSpan.FromMinutes(1).ToString());

            //var pagesCount = _memoryCache.GetOrCreateExclusive(cacheKey, (cacheEntry) => {

            //    return CountContentItemsRecursive(GetContentBasePath("pages", storeId), contentStorageProvider, GetContentBasePath("blogs", storeId));

            //});

            var storeTask = _storeService.GetByIdAsync(storeId);
            var themesTask = contentStorageProvider.SearchAsync(GetContentBasePath("themes", storeId), null);
            var blogsTask =  contentStorageProvider.SearchAsync(GetContentBasePath("blogs", storeId), null);

            await Task.WhenAll(storeTask, themesTask, blogsTask);

            var store = storeTask.Result;
            var themes = themesTask.Result;
            var blogs = blogsTask.Result;

            var retVal = new ContentStatistic
            {
                ActiveThemeName = store.GetDynamicPropertyValue("DefaultThemeName", "not set"),
                ThemesCount = themes.Results.Count(x => x.Type.EqualsInvariant("folder")),
                BlogsCount = themes.Results.Count(x => x.Type.EqualsInvariant("folder")),
                PagesCount = 10 //pagesCount
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
        [ProducesResponseType(200)]
        [Authorize(Permissions.Delete)]
        public async Task<IActionResult> DeleteContentAsync(string contentType, string storeId, [FromQuery] string[] urls)
        {
            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));

            await storageProvider.RemoveAsync(urls);

            //ToDo Reset cached items
            //_cacheManager.ClearRegion($"content-{storeId}");
            ContentCacheRegion.ExpireRegion();
            return Ok();
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
        [ProducesResponseType(typeof(byte[]), 200)]
        [Authorize(Permissions.Read)]

        public IActionResult GetContentItemDataStream(string contentType, string storeId, string relativeUrl)
        {
            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));
            var fileStream = storageProvider.OpenRead(relativeUrl);

            return File(fileStream, MimeTypeResolver.ResolveContentType(relativeUrl));
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
        [ProducesResponseType(typeof(ContentItem[]), 200)]
        [Authorize(Permissions.Read)]
        public async Task<IActionResult> SearchContentAsync(string contentType, string storeId, string folderUrl = null, string keyword = null)
        {
            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));

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
        [ProducesResponseType(200)]
        [Authorize(Permissions.Update)]
        public IActionResult MoveContent(string contentType, string storeId, string oldUrl, string newUrl)
        {
            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));

            storageProvider.MoveContent(oldUrl, newUrl);
            return Ok();
        }

        /// <summary>
        /// Copy contents
        /// </summary>
        /// <param name="srcPath">source content  relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/content/copy")]
        [ProducesResponseType(200)]
        [Authorize(Permissions.Update)]
        public IActionResult CopyContent(string srcPath, string destPath)
        {
            //This method used only for default themes copying that we use string.Empty instead storeId because default themes placed only in root content folder
            var storageProvider = _contentStorageProviderFactory(string.Empty);

            storageProvider.CopyContent(srcPath, destPath);
            return Ok();
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
        [ProducesResponseType(200)]
        [Authorize(Permissions.Update)]
        public async Task<IActionResult> UnpackAsync(string contentType, string storeId, string archivePath, string destPath)
        {
            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));

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

            return Ok();
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
        [ProducesResponseType(200)]
        [Authorize(Permissions.Create)]
        public async Task<IActionResult> CreateContentFolderAsync(string contentType, string storeId, ContentFolder folder)
        {
            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));

            await storageProvider.CreateFolderAsync(folder.ToBlobModel(AbstractTypeFactory<BlobFolder>.TryCreateInstance()));

            return Ok();
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
        [ProducesResponseType(typeof(ContentItem[]), 200)]
        [Authorize(Permissions.Create)]
        public async Task<IActionResult> UploadContent(string contentType, string storeId, [FromQuery] string folderUrl, [FromQuery]string url = null)
        {
            if (url == null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var retVal = new List<ContentFile>();

            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));

            if (url != null)
            {
                var fileName = HttpUtility.UrlDecode(System.IO.Path.GetFileName(url));
                var fileUrl = folderUrl + "/" + fileName;

                using (var client = new WebClient())
                using (var blobStream = storageProvider.OpenWrite(fileUrl))
                using (var remoteStream = client.OpenRead(url))
                {
                    remoteStream.CopyTo(blobStream);

                    var сontentFile = AbstractTypeFactory<ContentFile>.TryCreateInstance();

                    сontentFile.Name = fileName;
                    сontentFile.Url = _urlResolver.GetAbsoluteUrl(fileUrl);
                    retVal.Add(сontentFile);
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
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                    if (hasContentDispositionHeader)
                    {
                        if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                        {
                            var fileName = contentDisposition.FileName.Value;

                            targetFilePath = folderUrl + "/" + fileName;

                            using (var targetStream = storageProvider.OpenWrite(targetFilePath))
                            {
                                await section.Body.CopyToAsync(targetStream);
                            }

                            var contentFile = AbstractTypeFactory<ContentFile>.TryCreateInstance();
                            contentFile.Name = fileName;
                            contentFile.Url = _urlResolver.GetAbsoluteUrl(targetFilePath);
                            retVal.Add(contentFile);
                        }
                    }
                }
            }

            //ToDo Reset cached items
            //_cacheManager.ClearRegion($"content-{storeId}");
            ContentCacheRegion.ExpireRegion();

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

        private int CountContentItemsRecursive(string folderUrl, IContentBlobStorageProvider _contentStorageProvider, string excludedFolderUrl = null)
        {
            var searchResult = _contentStorageProvider.SearchAsync(folderUrl, null).GetAwaiter().GetResult();
            var retVal = searchResult.TotalCount
                        + searchResult.Results.OfType<BlobFolder>()
                            .Where(x => excludedFolderUrl == null || !x.RelativeUrl.EndsWith(excludedFolderUrl, StringComparison.InvariantCultureIgnoreCase))
                            .Select(x => CountContentItemsRecursive(x.RelativeUrl, _contentStorageProvider))
                            .Sum();

            return retVal;
        }
    }
}
