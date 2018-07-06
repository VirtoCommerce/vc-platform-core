using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.SitemapsModule.Data.Services.SitemapItemRecordProviders
{
    public class VendorSitemapItemRecordProvider : SitemapItemRecordProviderBase, ISitemapItemRecordProvider
    {
        public VendorSitemapItemRecordProvider(
            ISitemapUrlBuilder urlBuilder,
            ISettingsManager settingsManager,
            IMemberService memberService)
            : base(settingsManager, urlBuilder)
        {
            MemberService = memberService;
        }

        protected IMemberService MemberService { get; private set; }

        public virtual void LoadSitemapItemRecords(Store store, Sitemap sitemap, string baseUrl, Action<ExportImportProgressInfo> progressCallback = null)
        {
            var progressInfo = new ExportImportProgressInfo();

            var sitemapItemRecords = new List<SitemapItemRecord>();
            var vendorOptions = new SitemapItemOptions();

            var vendorSitemapItems = sitemap.Items.Where(x => x.ObjectType.EqualsInvariant(SitemapItemTypes.Vendor));
            var vendorIds = vendorSitemapItems.Select(x => x.ObjectId).ToArray();
            var members = MemberService.GetByIds(vendorIds);

            var totalCount = members.Count();
            var processedCount = 0;
            progressInfo.Description = $"Vendor: start generating {totalCount} records for vendors";
            progressCallback?.Invoke(progressInfo);

            foreach (var sitemapItem in vendorSitemapItems)
            {
                var vendor = members.FirstOrDefault(x => x.Id == sitemapItem.ObjectId) as Vendor;
                if (vendor != null)
                {
                    sitemapItem.ItemsRecords = GetSitemapItemRecords(store, vendorOptions, sitemap.UrlTemplate, baseUrl, vendor);

                    processedCount++;
                    progressInfo.Description = $"Vendor: generated  {processedCount} of {totalCount} records for vendors";
                    progressCallback?.Invoke(progressInfo);
                }
            }
        }
    }
}
