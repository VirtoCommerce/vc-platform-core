using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.SitemapsModule.Data.Services.SitemapItemRecordProviders
{
    //ToDo dependency Customer
    //public class VendorSitemapItemRecordProvider : SitemapItemRecordProviderBase, ISitemapItemRecordProvider
    //{
    //    protected IMemberService _memberService;

    //    public VendorSitemapItemRecordProvider(ISitemapUrlBuilder urlBuilder, ISettingsManager settingsManager, IMemberService memberService)
    //        : base(settingsManager, urlBuilder)
    //    {
    //        _memberService = memberService;
    //    }



    //    public virtual void LoadSitemapItemRecordsAsync(Store store, Sitemap sitemap, string baseUrl, Action<ExportImportProgressInfo> progressCallback = null)
    //    {
    //        var progressInfo = new ExportImportProgressInfo();

    //        var sitemapItemRecords = new List<SitemapItemRecord>();
    //        var vendorOptions = new SitemapItemOptions();

    //        var vendorSitemapItems = sitemap.Items.Where(x => x.ObjectType.EqualsInvariant(SitemapItemTypes.Vendor));
    //        var vendorIds = vendorSitemapItems.Select(x => x.ObjectId).ToArray();
    //        var members = _memberService.GetByIds(vendorIds);

    //        var totalCount = members.Count();
    //        var processedCount = 0;
    //        progressInfo.Description = $"Vendor: start generating {totalCount} records for vendors";
    //        progressCallback?.Invoke(progressInfo);

    //        foreach (var sitemapItem in vendorSitemapItems)
    //        {
    //            var vendor = members.FirstOrDefault(x => x.Id == sitemapItem.ObjectId) as Vendor;
    //            if (vendor != null)
    //            {
    //                sitemapItem.ItemsRecords = GetSitemapItemRecords(store, vendorOptions, sitemap.UrlTemplate, baseUrl, vendor);

    //                processedCount++;
    //                progressInfo.Description = $"Vendor: generated  {processedCount} of {totalCount} records for vendors";
    //                progressCallback?.Invoke(progressInfo);
    //            }
    //        }
    //    }
    //}
}
