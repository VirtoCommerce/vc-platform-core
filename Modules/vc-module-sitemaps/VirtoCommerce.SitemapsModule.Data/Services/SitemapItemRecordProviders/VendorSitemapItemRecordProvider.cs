using System;
using System.Linq;
using System.Threading.Tasks;
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
        public VendorSitemapItemRecordProvider(
            ISitemapUrlBuilder urlBuilder,
            ISettingsManager settingsManager,
            IMemberService memberService)
            : base(settingsManager, urlBuilder)
        {
            MemberService = memberService;
        }

        protected IMemberService MemberService { get; }

        public virtual async Task LoadSitemapItemRecordsAsync(Store store, Sitemap sitemap, string baseUrl, Action<ExportImportProgressInfo> progressCallback = null)
        {
            var vendorOptions = new SitemapItemOptions();
            var vendorSitemapItems = sitemap.Items.Where(x => x.ObjectType.EqualsInvariant(SitemapItemTypes.Vendor))
                                                  .ToList();

            var vendorIds = vendorSitemapItems.Select(x => x.ObjectId).ToArray();
            var members = await MemberService.GetByIdsAsync(vendorIds);

            var progressInfo = GetProgressInfo(progressCallback, members.Length);
            progressInfo.Start();

            foreach (var member in members)
            {
                var sitemapItem = vendorSitemapItems.First(x => x.ObjectId == member.Id);
                sitemapItem.ItemsRecords = GetSitemapItemRecords(store, vendorOptions, sitemap.UrlTemplate, baseUrl, member);
                progressInfo.Next();
            }

            progressInfo.End();
        }

        private SitemapProgressInfo GetProgressInfo(Action<ExportImportProgressInfo> progressCallback, long totalCount)
        {
            return new SitemapProgressInfo
            {
                StartDescriptionTemplate = "Vendor: start generating records for {0} vendors",
                EndDescriptionTemplate = "Vendor:  {0} vendors generated",
                ProgressDescriptionTemplate = "Vendor: generated records for {0} of {1} vendors",
                ProgressCallback = progressCallback,
                TotalCount = totalCount
            };
        }
    }
}
