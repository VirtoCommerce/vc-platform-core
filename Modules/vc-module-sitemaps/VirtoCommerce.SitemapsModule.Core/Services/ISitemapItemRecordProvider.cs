using System;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.SitemapsModule.Core.Services
{
    public interface ISitemapItemRecordProvider
    {
        void LoadSitemapItemRecordsAsync(Store store, Sitemap sitemap, string baseUrl, Action<ExportImportProgressInfo> progressCallback = null);
    }
}
