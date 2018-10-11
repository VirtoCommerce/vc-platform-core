using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
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
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Services;
using VirtoCommerce.SitemapsModule.Web.Model.PushNotifications;
using VirtoCommerce.SitemapsModule.Web.Security;
using SystemFile = System.IO.File;

namespace VirtoCommerce.SitemapsModule.Web.Controllers.Api
{
    [Route("api/sitemaps")]
    [Authorize(SitemapsPredefinedPermissions.Read)]
    public class SitemapsModuleApiController : Controller
    {
        private readonly ISitemapService _sitemapService;
        private readonly ISitemapItemService _sitemapItemService;
        private readonly ISitemapXmlGenerator _sitemapXmlGenerator;
        private readonly IUserNameResolver _userNameResolver;
        private readonly IPushNotificationManager _notifier;
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IBlobUrlResolver _blobUrlResolver;
        private readonly IHostingEnvironment _hostingEnvironment;

        public SitemapsModuleApiController(
            ISitemapService sitemapService,
            ISitemapItemService sitemapItemService,
            ISitemapXmlGenerator sitemapXmlGenerator,
            IUserNameResolver userNameResolver,
            IPushNotificationManager notifier,
            IBlobStorageProvider blobStorageProvider,
            IBlobUrlResolver blobUrlResolver,
            IHostingEnvironment hostingEnvironment)
        {
            _sitemapService = sitemapService;
            _sitemapItemService = sitemapItemService;
            _sitemapXmlGenerator = sitemapXmlGenerator;
            _userNameResolver = userNameResolver;
            _notifier = notifier;
            _blobStorageProvider = blobStorageProvider;
            _blobUrlResolver = blobUrlResolver;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(GenericSearchResult<Sitemap>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> SearchSitemaps(SitemapSearchCriteria request)
        {
            if (request == null)
            {
                return BadRequest("request is null");
            }

            var sitemapSearchResponse = await _sitemapService.SearchAsync(request);

            return Ok(sitemapSearchResponse);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Sitemap), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetSitemapById(string id)
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

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Sitemap), 200)]
        [Authorize(SitemapsPredefinedPermissions.Create)]
        public async Task<IActionResult> AddSitemap(Sitemap sitemap)
        {
            if (sitemap == null)
            {
                return BadRequest("sitemap is null");
            }

            await _sitemapService.SaveChangesAsync(new[] { sitemap });

            return Ok(sitemap);
        }

        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(void), 204)]
        [Authorize(SitemapsPredefinedPermissions.Update)]
        public async Task<IActionResult> UpdateSitemap(Sitemap sitemap)
        {
            if (sitemap == null)
            {
                return BadRequest("sitemap is null");
            }

            await _sitemapService.SaveChangesAsync(new[] { sitemap });

            return NoContent();
        }

        [HttpDelete]
        [Route("")]
        [ProducesResponseType(typeof(void), 204)]
        [Authorize(SitemapsPredefinedPermissions.Delete)]
        public async Task<IActionResult> DeleteSitemap(string[] ids)
        {
            if (ids == null)
            {
                return BadRequest("ids is null");
            }

            await _sitemapService.RemoveAsync(ids);

            return NoContent();
        }

        [HttpPost]
        [Route("items/search")]
        [ProducesResponseType(typeof(GenericSearchResult<SitemapItem>), 200)]
        public async Task<IActionResult> SearchSitemapItems(SitemapItemSearchCriteria request)
        {
            if (request == null)
            {
                return BadRequest("request is null");
            }

            var searchSitemapItemResponse = await _sitemapItemService.SearchAsync(request);

            return Ok(searchSitemapItemResponse);
        }

        [HttpPost]
        [Route("{sitemapId}/items")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> AddSitemapItems(string sitemapId, [FromBody]SitemapItem[] items)
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

            return Ok();
        }

        [HttpDelete]
        [Route("items")]
        [ProducesResponseType(typeof(void), 204)]
        public async Task<IActionResult> RemoveSitemapItems(string[] itemIds)
        {
            if (itemIds == null)
            {
                return BadRequest("itemIds is null");
            }

            await _sitemapItemService.RemoveAsync(itemIds);

            return NoContent();
        }

        [HttpGet]
        [Route("schema")]
        [ProducesResponseType(typeof(string[]), 200)]
        public async Task<IActionResult> GetSitemapsSchema(string storeId)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return BadRequest("storeId is empty");
            }

            var sitemapUrls = await _sitemapXmlGenerator.GetSitemapUrlsAsync(storeId);
            return Ok(sitemapUrls);
        }

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

        [HttpGet]
        [Route("download")]
        [ProducesResponseType(typeof(SitemapDownloadNotification), 200)]
        public async Task<IActionResult> DownloadSitemap(string storeId, string baseUrl)
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

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task BackgroundDownload(string storeId, string baseUrl, SitemapDownloadNotification notification)
        {
            Action<ExportImportProgressInfo> progressCallback = c =>
            {
                // TODO: is there a better way to copy ExportImportProgressInfo properties to SitemapDownloadNotification without using ValueInjecter?
                notification.Description = c.Description;
                notification.ProcessedCount = c.ProcessedCount;
                notification.TotalCount = c.TotalCount;
                notification.Errors = c.Errors?.ToList() ?? new List<string>();

                _notifier.Send(notification);
            };

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
                    using (var zipPackage = ZipPackage.Open(stream, FileMode.Create))
                    {
                        await CreateSitemapPartAsync(zipPackage, storeId, baseUrl, "sitemap.xml", progressCallback);

                        var sitemapUrls = await _sitemapXmlGenerator.GetSitemapUrlsAsync(storeId);
                        foreach (var sitemapUrl in sitemapUrls)
                        {
                            if (!string.IsNullOrEmpty(sitemapUrl))
                            {
                                await CreateSitemapPartAsync(zipPackage, storeId, baseUrl, sitemapUrl, progressCallback);
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

        private async Task CreateSitemapPartAsync(Package package, string storeId, string baseUrl, string sitemapUrl, Action<ExportImportProgressInfo> progressCallback)
        {
            var uri = PackUriHelper.CreatePartUri(new Uri(sitemapUrl, UriKind.Relative));
            var sitemapPart = package.CreatePart(uri, System.Net.Mime.MediaTypeNames.Text.Xml, CompressionOption.Normal);
            var stream = await _sitemapXmlGenerator.GenerateSitemapXmlAsync(storeId, baseUrl, sitemapUrl, progressCallback);
            var sitemapPartStream = sitemapPart.GetStream();
            stream.CopyTo(sitemapPartStream);
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
