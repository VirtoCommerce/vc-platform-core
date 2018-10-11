using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;

namespace VirtoCommerce.SitemapsModule.Web.ExportImport
{
    public sealed class BackupObject
    {
        public BackupObject()
        {
            Sitemaps = new List<Sitemap>();
        }

        public ICollection<Sitemap> Sitemaps { get; set; }
        public ICollection<SitemapItem> SitemapItems { get; set; }
    }

    public sealed class SitemapExportImport
    {
        private readonly ISitemapService _sitemapService;
        private readonly ISitemapItemService _sitemapItemService;

        public SitemapExportImport(ISitemapService sitemapService, ISitemapItemService sitemapItemService)
        {
            _sitemapService = sitemapService;
            _sitemapItemService = sitemapItemService;
        }

        public async Task DoExportAsync(Stream backupStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            var backupObject = await GetBackupObject(progressCallback, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            backupObject.SerializeJson(backupStream);
        }

        public async Task DoImportAsync(Stream backupStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var backupObject = backupStream.DeserializeJson<BackupObject>();
            var progressInfo = new ExportImportProgressInfo();

            cancellationToken.ThrowIfCancellationRequested();

            progressInfo.Description = "Sitemaps importing...";
            progressCallback(progressInfo);
            await _sitemapService.SaveChangesAsync(backupObject.Sitemaps.ToArray());

            cancellationToken.ThrowIfCancellationRequested();

            progressInfo.Description = "Sitemaps items importing...";
            progressCallback(progressInfo);
            await _sitemapItemService.SaveChangesAsync(backupObject.SitemapItems.ToArray());
        }

        private async Task<BackupObject> GetBackupObject(Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var backupObject = new BackupObject();
            var progressInfo = new ExportImportProgressInfo();

            progressInfo.Description = "Sitemaps loading...";
            progressCallback(progressInfo);
            //Load sitemaps
            var sitemapSearchCriteria = new SitemapSearchCriteria {
                Skip = 0,
                Take = int.MaxValue
            };
            var sitemapSearchResult = await _sitemapService.SearchAsync(sitemapSearchCriteria);
            backupObject.Sitemaps = sitemapSearchResult.Results;

            cancellationToken.ThrowIfCancellationRequested();

            progressInfo.Description = "Sitemaps items loading...";
            progressCallback(progressInfo);
            var sitemapItemsSearchCriteria = new SitemapItemSearchCriteria
            {
                Skip = 0,
                Take = int.MaxValue
            };
            var sitemapItemsSearchResult = await _sitemapItemService.SearchAsync(sitemapItemsSearchCriteria);
            backupObject.SitemapItems = sitemapItemsSearchResult.Results;

            return backupObject;
        } 
      
    }
}
