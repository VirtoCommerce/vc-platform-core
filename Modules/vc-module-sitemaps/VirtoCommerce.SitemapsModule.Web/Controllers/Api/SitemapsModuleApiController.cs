using System;
using System.IO;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Models.PushNotifications;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Services;

using Permission = VirtoCommerce.SitemapsModule.Core.SitemapsConstants.Security.Permissions;

namespace VirtoCommerce.SitemapsModule.Web.Controllers.Api
{
    [Route("api/sitemaps")]
    [Authorize(Permission.Read)]
    public class SitemapsModuleApiController : Controller
    {
        private readonly ISitemapService _sitemapService;
        private readonly ISitemapItemService _sitemapItemService;
        private readonly ISitemapXmlGenerator _sitemapXmlGenerator;
        private readonly IUserNameResolver _userNameResolver;
        private readonly IPushNotificationManager _notifier;
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public SitemapsModuleApiController(
            ISitemapService sitemapService,
            ISitemapItemService sitemapItemService,
            ISitemapXmlGenerator sitemapXmlGenerator,
            IUserNameResolver userNameResolver,
            IPushNotificationManager notifier,
            IBlobStorageProvider blobStorageProvider,
            IBlobUrlResolver blobUrlResolver)
        {
            _sitemapService = sitemapService;
            _sitemapItemService = sitemapItemService;
            _sitemapXmlGenerator = sitemapXmlGenerator;
            _userNameResolver = userNameResolver;
            _notifier = notifier;
            _blobStorageProvider = blobStorageProvider;
            _blobUrlResolver = blobUrlResolver;
        }

        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(GenericSearchResult<Sitemap>), 200)]
        [ProducesResponseType(400)]
        public IActionResult SearchSitemaps(SitemapSearchCriteria request)
        {
            if (request == null)
            {
                return BadRequest("request is null");
            }

            var sitemapSearchResponse = _sitemapService.SearchAsync(request);

            return Ok(sitemapSearchResponse);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Sitemap), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetSitemapById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("id is null");
            }

            var sitemap = _sitemapService.GetByIdAsync(id);

            if (sitemap == null)
            {
                return NotFound();
            }

            return Ok(sitemap);
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Sitemap), 200)]
        [Authorize(Permission.Create)]
        public IActionResult AddSitemap(Sitemap sitemap)
        {
            if (sitemap == null)
                return BadRequest("sitemap is null");

            _sitemapService.SaveChangesAsync(new[] { sitemap });

            return Ok(sitemap);
        }

        [HttpPut]
        [Route("")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Authorize(Permission.Update)]
        public IActionResult UpdateSitemap(Sitemap sitemap)
        {
            if (sitemap == null)
                return BadRequest("sitemap is null");

            _sitemapService.SaveChangesAsync(new[] { sitemap });

            return Ok();
        }

        [HttpDelete]
        [Route("")]
        [ProducesResponseType(200)]
        [Authorize(Permission.Delete)]
        public IActionResult DeleteSitemap([FromQuery]string[] ids)
        {
            if (ids == null)
            {
                return BadRequest("ids is null");
            }

            _sitemapService.RemoveAsync(ids);

            return Ok();
        }

        [HttpPost]
        [Route("items/search")]
        [ProducesResponseType(typeof(GenericSearchResult<SitemapItem>), 200)]
        [ProducesResponseType(400)]
        public IActionResult SearchSitemapItems([FromBody]SitemapItemSearchCriteria request)
        {
            if (request == null)
            {
                return BadRequest("request is null");
            }

            var searchSitemapItemResponse = _sitemapItemService.SearchAsync(request);

            return Ok(searchSitemapItemResponse);
        }

        [HttpPost]
        [Route("{sitemapId}/items")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult AddSitemapItems([FromRoute]string sitemapId, [FromBody]SitemapItem[] items)
        {
            if (string.IsNullOrEmpty(sitemapId))
            {
                return BadRequest();
            }

            if (items == null)
            {
                return BadRequest("items is null");
            }

            foreach (var item in items)
            {
                item.SitemapId = sitemapId;
            }

            _sitemapItemService.SaveChangesAsync(items);

            return Ok();
        }

        [HttpDelete]
        [Route("items")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult RemoveSitemapItems([FromQuery]string[] itemIds)
        {
            if (itemIds == null)
            {
                return BadRequest("itemIds is null");
            }

            _sitemapItemService.RemoveAsync(itemIds);

            return Ok();
        }

        [HttpGet]
        [Route("schema")]
        [ProducesResponseType(typeof(string[]), 200)]
        [ProducesResponseType(400)]
        public IActionResult GetSitemapsSchema(string storeId)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return BadRequest("storeId is empty");
            }

            var sitemapUrls = _sitemapXmlGenerator.GetSitemapUrlsAsync(storeId);

            return Ok(sitemapUrls);
        }

        [HttpGet]
        [Route("generate")]
        [SwaggerFileResponse]
        [ProducesResponseType(typeof(byte[]), 200)]
        public async Task<IActionResult> GenerateSitemapAsync(string storeId, string baseUrl, string sitemapUrl)
        {
            var stream = await _sitemapXmlGenerator.GenerateSitemapXmlAsync(storeId, baseUrl, sitemapUrl);
            return File(stream, "text/xml");
        }

        [HttpGet]
        [Route("download")]
        [ProducesResponseType(typeof(SitemapDownloadNotification), 200)]
        public async Task<IActionResult> DownloadSitemapAsync(string storeId, string baseUrl)
        {
            var notification = new SitemapDownloadNotification(_userNameResolver.GetCurrentUserName())
            {
                Title = "Download sitemaps",
                Description = "Processing download sitemaps..."
            };

            await _notifier.SendAsync(notification);

            BackgroundJob.Enqueue(() => BackgroundDownloadAsync(storeId, baseUrl, notification));

            return Ok(notification);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task BackgroundDownloadAsync(string storeId, string baseUrl, SitemapDownloadNotification notification)
        {
            Action<ExportImportProgressInfo> progressCallback = async c =>
            {
                notification.Path(c);
                await _notifier.SendAsync(notification);
            };

            try
            {
                var relativeUrl = $"tmp/sitemap-{storeId}.zip";
                var localTmpFolder = HostingEnvironment.MapPath("~/App_Data/Uploads/tmp");
                var localTmpPath = Path.Combine(localTmpFolder, $"sitemap-{storeId}.zip");
                if (!Directory.Exists(localTmpFolder))
                {
                    Directory.CreateDirectory(localTmpFolder);
                }

                //Import first to local tmp folder because Azure blob storage doesn't support some special file access mode 
                using (var stream = System.IO.File.Open(localTmpPath, FileMode.OpenOrCreate))
                {
                    using (var zipPackage = ZipPackage.Open(stream, FileMode.Create))
                    {
                        CreateSitemapPart(zipPackage, storeId, baseUrl, "sitemap.xml", progressCallback);

                        var sitemapUrls = await _sitemapXmlGenerator.GetSitemapUrlsAsync(storeId);
                        foreach (var sitemapUrl in sitemapUrls)
                        {
                            if (!string.IsNullOrEmpty(sitemapUrl))
                            {
                                CreateSitemapPart(zipPackage, storeId, baseUrl, sitemapUrl, progressCallback);
                            }
                        }
                    }
                }
                //Copy export data to blob provider for get public download url
                using (var localStream = System.IO.File.Open(localTmpPath, FileMode.Open))
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

        private void CreateSitemapPart(Package package, string storeId, string baseUrl, string sitemapUrl, Action<ExportImportProgressInfo> progressCallback)
        {
            var uri = PackUriHelper.CreatePartUri(new Uri(sitemapUrl, UriKind.Relative));
            var sitemapPart = package.CreatePart(uri, System.Net.Mime.MediaTypeNames.Text.Xml, CompressionOption.Normal);
            var stream = _sitemapXmlGenerator.GenerateSitemapXml(storeId, baseUrl, sitemapUrl, progressCallback);
            var sitemapPartStream = sitemapPart.GetStream();
            stream.CopyTo(sitemapPartStream);
        }
    }
}
