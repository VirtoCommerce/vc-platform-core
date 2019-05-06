using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Data.ExportImport;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Models.Search;
using VirtoCommerce.SitemapsModule.Core.Services;

namespace VirtoCommerce.SitemapsModule.Data.ExportImport
{
    public sealed class SitemapExportImport
    {
        private readonly ISitemapService _sitemapService;
        private readonly ISitemapItemService _sitemapItemService;
        private readonly ISitemapSearchService _sitemapSearchService;
        private readonly ISitemapItemSearchService _sitemapItemSearchService;
        private const int _batchSize = 50;
        private readonly JsonSerializer _jsonSerializer;

        public SitemapExportImport(ISitemapService sitemapService, ISitemapItemService sitemapItemService, ISitemapSearchService sitemapSearchService, ISitemapItemSearchService sitemapItemSearchService, JsonSerializer jsonSerializer)
        {
            _sitemapService = sitemapService;
            _sitemapItemService = sitemapItemService;
            _sitemapSearchService = sitemapSearchService;
            _sitemapItemSearchService = sitemapItemSearchService;
            _jsonSerializer = jsonSerializer;
        }

        public async Task DoExportAsync(Stream outStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream))
            using (var writer = new JsonTextWriter(sw))
            {
                await writer.WriteStartObjectAsync();

                progressInfo.Description = "Site maps exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Sitemaps");

                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) => (GenericSearchResult<Sitemap>)await _sitemapSearchService.SearchAsync(new SitemapSearchCriteria { Skip = skip, Take = take }), (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } site maps have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Site map items exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("SitemapItems");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) => (GenericSearchResult<SitemapItem>)await _sitemapItemSearchService.SearchAsync(new SitemapItemSearchCriteria { Skip = skip, Take = take }), (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } site maps items have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                await writer.WriteEndObjectAsync();
                await writer.FlushAsync();
            }
        }

        public async Task DoImportAsync(Stream inputStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();

            using (var streamReader = new StreamReader(inputStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value.ToString() == "Sitemaps")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<Sitemap>(_jsonSerializer, _batchSize, items => _sitemapService.SaveChangesAsync(items.ToArray()), processedCount =>
                             {
                                 progressInfo.Description = $"{ processedCount } site maps have been imported";
                                 progressCallback(progressInfo);
                             }, cancellationToken);

                        }
                        else if (reader.Value.ToString() == "SitemapItems")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<SitemapItem>(_jsonSerializer, _batchSize, items => _sitemapItemService.SaveChangesAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } site maps items have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                    }
                }
            }

        }
    }
}
