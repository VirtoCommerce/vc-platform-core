using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.SitemapsModule.Data.Services.SitemapItemRecordProviders
{
    public class VendorSitemapItemRecordProvider : SitemapItemRecordProviderBase, ISitemapItemRecordProvider
    {
        private readonly IMemberService _memberService;


        public VendorSitemapItemRecordProvider(
            ISitemapUrlBuilder urlBuilder,
            ISettingsManager settingsManager,
            IMemberService memberService)
            : base(settingsManager, urlBuilder)
        {
            _memberService = memberService;
        }


        #region ISitemapItemRecordProvider members
        public virtual async Task LoadSitemapItemRecordsAsync(Store store, Sitemap sitemap, string baseUrl, Action<ExportImportProgressInfo> progressCallback = null)
        {
            var progressInfo = new ExportImportProgressInfo();

            var sitemapItemRecords = new List<SitemapItemRecord>();
            var vendorOptions = new SitemapItemOptions();

            var vendorSitemapItems = sitemap.Items.Where(x => x.ObjectType.EqualsInvariant(SitemapItemTypes.Vendor)).ToList();

            if (vendorSitemapItems.Count > 0)
            {
                var vendorIds = vendorSitemapItems.Select(x => x.ObjectId).ToArray();
                var members = await _memberService.GetByIdsAsync(vendorIds);

                var totalCount = members.Length;
                var processedCount = 0;
                progressInfo.Description = $"Vendor: Starting records generation for {totalCount} vendors";
                progressCallback?.Invoke(progressInfo);

                foreach (var sitemapItem in vendorSitemapItems)
                {
                    var vendor = members.FirstOrDefault(x => x.Id == sitemapItem.ObjectId) as Vendor;
                    if (vendor != null)
                    {
                        sitemapItem.ItemsRecords = GetSitemapItemRecords(store, vendorOptions, sitemap.UrlTemplate, baseUrl, vendor);

                        processedCount++;
                        progressInfo.Description = $"Vendor: Have been generated  {processedCount} of {totalCount} records for vendors items";
                        progressCallback?.Invoke(progressInfo);
                    }
                }
            }
        }
        #endregion
    }
}
