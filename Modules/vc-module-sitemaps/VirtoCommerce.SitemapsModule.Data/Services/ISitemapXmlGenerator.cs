using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.SitemapsModule.Data.Services
{
    public interface ISitemapXmlGenerator
    {
        ICollection<string> GetSitemapUrlsAsync(string storeId);

        Stream GenerateSitemapXmlAsync(string storeId, string baseUrl, string sitemapUrl, Action<ExportImportProgressInfo> progressCallback = null);
    }
}
