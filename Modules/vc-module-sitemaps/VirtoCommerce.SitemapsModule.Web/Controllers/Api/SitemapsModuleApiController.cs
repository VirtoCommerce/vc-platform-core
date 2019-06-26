using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.SitemapsModule.Core;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Models.Search;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Services;
using VirtoCommerce.SitemapsModule.Web.Model.PushNotifications;
using SystemFile = System.IO.File;

namespace VirtoCommerce.SitemapsModule.Web.Controllers.Api
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/sitemaps")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public class SitemapsModuleApiController : Controller
    {
        private readonly ISitemapService _sitemapService;
        private readonly ISitemapItemService _sitemapItemService;
        private readonly ISitemapSearchService _sitemapSearchService;
        private readonly ISitemapItemSearchService _sitemapItemSearchService;
        private readonly ISitemapXmlGenerator _sitemapXmlGenerator;
        private readonly IUserNameResolver _userNameResolver;
        private readonly IPushNotificationManager _notifier;
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IBlobUrlResolver _blobUrlResolver;
        private readonly IHostingEnvironment _hostingEnvironment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sitemapService"></param>
        /// <param name="sitemapItemService"></param>
        /// <param name="sitemapSearchService"></param>
        /// <param name="sitemapItemSearchService"></param>
        /// <param name="sitemapXmlGenerator"></param>
        /// <param name="userNameResolver"></param>
        /// <param name="notifier"></param>
        /// <param name="blobStorageProvider"></param>
        /// <param name="blobUrlResolver"></param>
        /// <param name="hostingEnvironment"></param>
        public SitemapsModuleApiController(
            ISitemapService sitemapService,
            ISitemapItemService sitemapItemService,
            ISitemapSearchService sitemapSearchService,
            ISitemapItemSearchService sitemapItemSearchService,
            ISitemapXmlGenerator sitemapXmlGenerator,
            IUserNameResolver userNameResolver,
            IPushNotificationManager notifier,
            IBlobStorageProvider blobStorageProvider,
            IBlobUrlResolver blobUrlResolver,
            IHostingEnvironment hostingEnvironment)
        {
            _sitemapService = sitemapService;
            _sitemapItemService = sitemapItemService;
            _sitemapSearchService = sitemapSearchService;
            _sitemapItemSearchService = sitemapItemSearchService;
            _sitemapXmlGenerator = sitemapXmlGenerator;
            _userNameResolver = userNameResolver;
            _notifier = notifier;
            _blobStorageProvider = blobStorageProvider;
            _blobUrlResolver = blobUrlResolver;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<ActionResult<SitemapSearchResult>> SearchSitemaps([FromBody] SitemapSearchCriteria request)
        {
            if (request == null)
            {
                return BadRequest("request is null");
            }

            var sitemapSearchResponse = await _sitemapSearchService.SearchAsync(request);

            return Ok(sitemapSearchResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Sitemap>> GetSitemapById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("id is null");
            }

            var sitemap = await _sitemapService.GetByIdAsync(id);

            if (sitemap == null)
            {
                return NotFound();
            }

            return Ok(sitemap);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sitemap"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult> AddSitemap([FromBody]Sitemap sitemap)
        {
            if (sitemap == null)
            {
                return BadRequest("sitemap is null");
            }

            await _sitemapService.SaveChangesAsync(new[] { sitemap });

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sitemap"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult> UpdateSitemap([FromBody]Sitemap sitemap)
        {
            if (sitemap == null)
            {
                return BadRequest("sitemap is null");
            }

            await _sitemapService.SaveChangesAsync(new[] { sitemap });

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<ActionResult> DeleteSitemap(string[] ids)
        {
            if (ids == null)
            {
                return BadRequest("ids is null");
            }

            await _sitemapService.RemoveAsync(ids);

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("items/search")]
        public async Task<ActionResult<SitemapItemsSearchResult>> SearchSitemapItems([FromBody] SitemapItemSearchCriteria request)
        {
            if (request == null)
            {
                return BadRequest("request is null");
            }

            var result = await _sitemapItemSearchService.SearchAsync(request);

            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sitemapId"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{sitemapId}/items")]
        public async Task<ActionResult> AddSitemapItems(string sitemapId, [FromBody]SitemapItem[] items)
        {
            if (string.IsNullOrEmpty(sitemapId))
            {
                return BadRequest("sitemapId is null");
            }
            if (items == null)
            {
                return BadRequest("items is null");
            }

            foreach (var item in items)
            {
                item.SitemapId = sitemapId;
            }
            await _sitemapItemService.SaveChangesAsync(items);

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("items")]
        public async Task<ActionResult> RemoveSitemapItems(string[] itemIds)
        {
            if (itemIds == null)
            {
                return BadRequest("itemIds is null");
            }

            await _sitemapItemService.RemoveAsync(itemIds);

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("schema")]
        public async Task<ActionResult<string[]>> GetSitemapsSchema(string storeId)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return BadRequest("storeId is empty");
            }

            var sitemapUrls = await _sitemapXmlGenerator.GetSitemapUrlsAsync(storeId);
            return Ok(sitemapUrls);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="baseUrl"></param>
        /// <param name="sitemapUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("generate")]
        [SwaggerFileResponse]
        public async Task<HttpResponseMessage> GenerateSitemap(string storeId, string baseUrl, string sitemapUrl)
        {
            var stream = await _sitemapXmlGenerator.GenerateSitemapXmlAsync(storeId, baseUrl, sitemapUrl);

            var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("download")]
        public async Task<ActionResult<SitemapDownloadNotification>> DownloadSitemap(string storeId, string baseUrl)
        {
            var notification = new SitemapDownloadNotification(_userNameResolver.GetCurrentUserName())
            {
                Title = "Download sitemaps",
                Description = "Processing download sitemaps..."
            };

            await _notifier.SendAsync(notification);

            BackgroundJob.Enqueue(() => BackgroundDownload(storeId, baseUrl, notification));

            return Ok(notification);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="baseUrl"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task BackgroundDownload(string storeId, string baseUrl, SitemapDownloadNotification notification)
        {
            void SendNotificationWithProgressInfo(ExportImportProgressInfo c)
            {
                // TODO: is there a better way to copy ExportImportProgressInfo properties to SitemapDownloadNotification without using ValueInjecter?
                notification.Description = c.Description;
                notification.ProcessedCount = c.ProcessedCount;
                notification.TotalCount = c.TotalCount;
                notification.Errors = c.Errors?.ToList() ?? new List<string>();

                _notifier.Send(notification);
            }

            try
            {
                var relativeUrl = $"tmp/sitemap-{storeId}.zip";
                var localTmpFolder = MapPath(_hostingEnvironment, "~/App_Data/Uploads/tmp");
                var localTmpPath = Path.Combine(localTmpFolder, $"sitemap-{storeId}.zip");
                if (!Directory.Exists(localTmpFolder))
                {
                    Directory.CreateDirectory(localTmpFolder);
                }

                //Import first to local tmp folder because Azure blob storage doesn't support some special file access mode 
                using (var stream = SystemFile.Open(localTmpPath, FileMode.OpenOrCreate))
                {
                    using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true))
                    {
                        await CreateSitemapPartAsync(zipArchive, storeId, baseUrl, "sitemap.xml", SendNotificationWithProgressInfo);

                        var sitemapUrls = await _sitemapXmlGenerator.GetSitemapUrlsAsync(storeId);
                        foreach (var sitemapUrl in sitemapUrls)
                        {
                            if (!string.IsNullOrEmpty(sitemapUrl))
                            {
                                await CreateSitemapPartAsync(zipArchive, storeId, baseUrl, sitemapUrl, SendNotificationWithProgressInfo);
                            }
                        }
                    }
                }
                //Copy export data to blob provider for get public download url
                using (var localStream = SystemFile.Open(localTmpPath, FileMode.Open))
                using (var blobStream = _blobStorageProvider.OpenWrite(relativeUrl))
                {
                    localStream.CopyTo(blobStream);
                    notification.DownloadUrl = _blobUrlResolver.GetAbsoluteUrl(relativeUrl);
                    notification.Description = "Sitemap download finished";
                }
            }
            catch (Exception exception)
            {
                notification.Description = "Sitemap download failed";
                notification.Errors.Add(exception.ExpandExceptionMessage());
            }
            finally
            {
                notification.Finished = DateTime.UtcNow;
                await _notifier.SendAsync(notification);
            }
        }

        private async Task CreateSitemapPartAsync(ZipArchive zipArchive, string storeId, string baseUrl, string sitemapUrl, Action<ExportImportProgressInfo> progressCallback)
        {
            var sitemapPart = zipArchive.CreateEntry(sitemapUrl, CompressionLevel.Optimal);
            using (var sitemapPartStream = sitemapPart.Open())
            {
                var stream = await _sitemapXmlGenerator.GenerateSitemapXmlAsync(storeId, baseUrl, sitemapUrl, progressCallback);
                stream.CopyTo(sitemapPartStream);
            }
        }

        private static string MapPath(IHostingEnvironment hostEnv, string path)
        {
            // TECHDEBT: this method is copied from VC.Platform.Web.Extensions.HostingEnviromentExtension.
            //           It's probably better to use IPathMapper instead, once it'll be implemented somewhere.

            var result = hostEnv.WebRootPath;

            if (path.StartsWith("~/"))
            {
                result = Path.Combine(result, path.Replace("~/", string.Empty).Replace("/", "\\"));
            }
            else if (Path.IsPathRooted(path))
            {
                result = path;
            }

            return result;
        }
    }
}
