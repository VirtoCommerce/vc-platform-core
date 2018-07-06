using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Services;

namespace VirtoCommerce.SitemapsModule.Web.Controllers.Api
{
    [Route("api/sitemaps")]
    [CheckPermission(Permission = SitemapsPredefinedPermissions.Read)]
    public class SitemapsModuleApiController : ApiController
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
                return BadRequest();

            var sitemapSearchResponse = _sitemapService.Search(request);

            return Ok(sitemapSearchResponse);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Sitemap))]
        public IActionResult GetSitemapById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("id is null");
            }

            var sitemap = _sitemapService.GetById(id);

            if (sitemap == null)
            {
                return NotFound();
            }

            return Ok(sitemap);
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(void))]
        [CheckPermission(Permission = SitemapsPredefinedPermissions.Create)]
        public IActionResult AddSitemap(Sitemap sitemap)
        {
            if (sitemap == null)
            {
                return BadRequest("sitemap is null");
            }

            _sitemapService.SaveChanges(new[] { sitemap });

            return Ok(sitemap);
        }

        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(void))]
        [CheckPermission(Permission = SitemapsPredefinedPermissions.Update)]
        public IActionResult UpdateSitemap(Sitemap sitemap)
        {
            if (sitemap == null)
            {
                return BadRequest("sitemap is null");
            }

            _sitemapService.SaveChanges(new[] { sitemap });

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("")]
        [ProducesResponseType(typeof(void))]
        [CheckPermission(Permission = SitemapsPredefinedPermissions.Delete)]
        public IActionResult DeleteSitemap([FromUri]string[] ids)
        {
            if (ids == null)
            {
                return BadRequest("ids is null");
            }

            _sitemapService.Remove(ids);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("items/search")]
        [ProducesResponseType(typeof(GenericSearchResult<SitemapItem>))]
        public IActionResult SearchSitemapItems(SitemapItemSearchCriteria request)
        {
            if (request == null)
            {
                return BadRequest("request is null");
            }

            var searchSitemapItemResponse = _sitemapItemService.Search(request);

            return Ok(searchSitemapItemResponse);
        }

        [HttpPost]
        [Route("{sitemapId}/items")]
        [ProducesResponseType(typeof(void))]
        public IActionResult AddSitemapItems(string sitemapId, [FromBody]SitemapItem[] items)
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
            _sitemapItemService.SaveChanges(items);

            return Ok();
        }

        [HttpDelete]
        [Route("items")]
        [ProducesResponseType(typeof(void))]
        public IActionResult RemoveSitemapItems([FromUri]string[] itemIds)
        {
            if (itemIds == null)
            {
                return BadRequest("itemIds is null");
            }

            _sitemapItemService.Remove(itemIds);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("schema")]
        [ProducesResponseType(typeof(string[]))]
        public IActionResult GetSitemapsSchema(string storeId)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return BadRequest("storeId is empty");
            }

            var sitemapUrls = _sitemapXmlGenerator.GetSitemapUrls(storeId);

            return Ok(sitemapUrls);
        }

        [HttpGet]
        [Route("generate")]
        [SwaggerFileResponse]
        public HttpResponseMessage GenerateSitemap(string storeId, string baseUrl, string sitemapUrl)
        {
            var stream = _sitemapXmlGenerator.GenerateSitemapXml(storeId, baseUrl, sitemapUrl);

            var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

            return result;
        }

        [HttpGet]
        [Route("download")]
        [ProducesResponseType(typeof(SitemapDownloadNotification))]
        public IActionResult DownloadSitemap(string storeId, string baseUrl)
        {
            var notification = new SitemapDownloadNotification(_userNameResolver.GetCurrentUserName())
            {
                Title = "Download sitemaps",
                Description = "Processing download sitemaps..."
            };

            _notifier.Upsert(notification);

            BackgroundJob.Enqueue(() => BackgroundDownload(storeId, baseUrl, notification));

            return Ok(notification);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void BackgroundDownload(string storeId, string baseUrl, SitemapDownloadNotification notification)
        {
            Action<ExportImportProgressInfo> progressCallback = c =>
            {
                notification.InjectFrom(c);
                _notifier.Upsert(notification);
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
                using (var stream = File.Open(localTmpPath, FileMode.OpenOrCreate))
                {
                    using (var zipPackage = ZipPackage.Open(stream, FileMode.Create))
                    {
                        CreateSitemapPart(zipPackage, storeId, baseUrl, "sitemap.xml", progressCallback);

                        var sitemapUrls = _sitemapXmlGenerator.GetSitemapUrls(storeId);
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
                using (var localStream = File.Open(localTmpPath, FileMode.Open))
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
                _notifier.Upsert(notification);
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
