using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

using Permissions = VirtoCommerce.ContentModule.Core.ContentConstants.Security.Permissions;

namespace VirtoCommerce.ContentModule.Web.Controllers.Api
{
    [Route("api/content/{contentType}/{storeId}")]
    public class ContentController : Controller
    {
        private readonly Func<string, IContentBlobStorageProvider> _contentStorageProviderFactory;
        private readonly IBlobUrlResolver _urlResolver;
        private readonly IStoreService _storeService;
        private readonly ICacheManager<object> _cacheManager;

        public ContentController(Func<string, IContentBlobStorageProvider> contentStorageProviderFactory, IBlobUrlResolver urlResolver, ISecurityService securityService, IPermissionScopeService permissionScopeService, IStoreService storeService, ICacheManager<object> cacheManager)
        {
            _storeService = storeService;
            _contentStorageProviderFactory = contentStorageProviderFactory;
            _urlResolver = urlResolver;
            _cacheManager = cacheManager;
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
            var store = _storeService.GetById(storeId);

            var pagesCount = _cacheManager.Get("pagesCount", $"content-{storeId}", TimeSpan.FromMinutes(1), () =>
            {
                return CountContentItemsRecursive(GetContentBasePath("pages", storeId), contentStorageProvider, GetContentBasePath("blogs", storeId)); ;
            });

            var themes = await contentStorageProvider.SearchAsync(GetContentBasePath("themes", storeId), null);
            var blogs = await contentStorageProvider.SearchAsync(GetContentBasePath("blogs", storeId), null);

            var retVal = new ContentStatistic
            {
                ActiveThemeName = store.GetDynamicPropertyValue("DefaultThemeName", "not set"),
                ThemesCount = themes.Results.Count( x => x.Type.EqualsInvariant("folder")),
                BlogsCount = themes.Results.Count(x => x.Type.EqualsInvariant("folder")),
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
        [ProducesResponseType(200)]
        [Authorize(Permissions.Delete)]
        public async Task<IActionResult> DeleteContentAsync(string contentType, string storeId, [FromQuery] string[] urls)
        {
            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));

            await storageProvider.RemoveAsync(urls);
            _cacheManager.ClearRegion($"content-{storeId}");
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

        public HttpResponseMessage GetContentItemDataStream(string contentType, string storeId, string relativeUrl)
        {
            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));
            var stream = storageProvider.OpenRead(relativeUrl);
            var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(relativeUrl));
            return result;
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
            var retVal = result.Results.Where( x => x.Type.EqualsInvariant("folder")).Select(x => x.ToContentModel())
                               .OfType<ContentItem>()
                               .Concat(result.Results.Select(x => x.ToContentModel()))
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
        public IActionResult Unpack(string contentType, string storeId, string archivePath, string destPath)
        {
            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));

            using (var stream = storageProvider.OpenRead(archivePath))
            using (var archive = new ZipArchive(stream))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.FullName.EndsWith("/"))
                    {
                        var fileName = String.Join("/", entry.FullName.Split('/').Skip(1));
                        using (var entryStream = entry.Open())
                        using (var targetStream = storageProvider.OpenWrite(destPath + "/" + fileName))
                        {
                            entryStream.CopyTo(targetStream);
                        }
                    }
                }

            }
            //remove archive after unpack
            storageProvider.Remove(new[] { archivePath });
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
        public IActionResult CreateContentFolder(string contentType, string storeId, ContentFolder folder)
        {
            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));

            storageProvider.CreateFolderAsync(folder.ToBlobModel());
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
        [ProducesResponseType(typeof(ContentItem[]), 200)]
        [Authorize(Permissions.Create)]
        public async Task<IActionResult> UploadContent(string contentType, string storeId, [FromQuery] string folderUrl, [FromQuery]string url = null)
        {
            if (url == null && !Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
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

                    retVal.Add(new ContentFile
                    {
                        Name = fileName,
                        Url = _urlResolver.GetAbsoluteUrl(fileUrl)
                    });
                }
            }
            else
            {
                var blobMultipartProvider = new BlobStorageMultipartProvider(storageProvider, _urlResolver, folderUrl);
                await Request.Content.ReadAsMultipartAsync(blobMultipartProvider);

                var files = blobMultipartProvider.BlobInfos.Select(blobInfo => new ContentFile
                {
                    Name = blobInfo.FileName,
                    Url = _urlResolver.GetAbsoluteUrl(blobInfo.Key)
                });
                retVal.AddRange(files);
            }

            _cacheManager.ClearRegion($"content-{storeId}");
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
