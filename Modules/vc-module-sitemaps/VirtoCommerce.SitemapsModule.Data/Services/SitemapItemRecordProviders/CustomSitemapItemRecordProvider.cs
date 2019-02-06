using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.SitemapsModule.Data.Services.SitemapItemRecordProviders
{
    public class CustomSitemapItemRecordProvider : SitemapItemRecordProviderBase, ISitemapItemRecordProvider
    {
        public CustomSitemapItemRecordProvider(
            ISitemapUrlBuilder urlBuilder,
            ISettingsManager settingsManager)
            : base(settingsManager, urlBuilder)
        {
        }

        public virtual Task LoadSitemapItemRecordsAsync(Store store, Sitemap sitemap, string baseUrl, Action<ExportImportProgressInfo> progressCallback = null)
        {
            var progressInfo = new ExportImportProgressInfo();

            var sitemapItemRecords = new List<SitemapItemRecord>();
            var customOptions = new SitemapItemOptions();
            var customSitemapItems = sitemap.Items.Where(si => si.ObjectType.EqualsInvariant(SitemapItemTypes.Custom));
            var processedCount = 0;
            var totalCount = customSitemapItems.Count();
            progressInfo.Description = $"Custom: start generating {totalCount} custom records";
            progressCallback?.Invoke(progressInfo);

            foreach (var customSitemapItem in customSitemapItems)
            {
                customSitemapItem.ItemsRecords = GetSitemapItemRecords(store, customOptions, customSitemapItem.UrlTemplate, baseUrl);
                processedCount++;
                progressInfo.Description = $"Custom: generated {processedCount} of {totalCount} custom records";
                progressCallback?.Invoke(progressInfo);
            }

            return Task.CompletedTask;
        }
    }
}
