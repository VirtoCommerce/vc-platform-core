using System;
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
            var customOptions = new SitemapItemOptions();
            var customSitemapItems = sitemap.Items.Where(si => si.ObjectType.EqualsInvariant(SitemapItemTypes.Custom))
                                                  .ToList();

            var progressInfo = GetProgressInfo(progressCallback, customSitemapItems.Count);

            foreach (var customSitemapItem in customSitemapItems)
            {
                customSitemapItem.ItemsRecords = GetSitemapItemRecords(store, customOptions, customSitemapItem.UrlTemplate, baseUrl);
                progressInfo.Next();
            }

            progressInfo.End();
            return Task.CompletedTask;
        }

        private SitemapProgressInfo GetProgressInfo(Action<ExportImportProgressInfo> progressCallback, long totalCount)
        {
            return new SitemapProgressInfo
            {
                StartDescriptionTemplate = "Custom: start generating for {0} custom records",
                EndDescriptionTemplate = "Custom: {0} custom records generated",
                ProgressDescriptionTemplate = "Custom: generated {0} of {1} custom records",
                ProgressCallback = progressCallback,
                TotalCount = totalCount
            };
        }
    }
}
