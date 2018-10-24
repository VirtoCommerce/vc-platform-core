using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;

namespace VirtoCommerce.SitemapsModule.Data.ExportImport
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

        private readonly JsonSerializer _jsonSerializer;

        public SitemapExportImport(ISitemapService sitemapService, ISitemapItemService sitemapItemService,
            IOptions<MvcJsonOptions> mvcJsonOptions)
        {
            _sitemapService = sitemapService;
            _sitemapItemService = sitemapItemService;
            _jsonSerializer = JsonSerializer.Create(mvcJsonOptions.Value.SerializerSettings);
        }

        public async Task DoExportAsync(Stream backupStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();

            using (var textWriter = new StreamWriter(backupStream))
            using (var jsonTextWriter = new JsonTextWriter(textWriter))
            {
                await jsonTextWriter.WriteStartObjectAsync();

                progressInfo.Description = "Sitemaps loading...";
                progressCallback(progressInfo);

                //Load sitemaps
                var sitemapSearchCriteria = new SitemapSearchCriteria
                {
                    Skip = 0,
                    Take = int.MaxValue
                };
                var sitemapSearchResult = await _sitemapService.SearchAsync(sitemapSearchCriteria);

                await jsonTextWriter.WritePropertyNameAsync("Sitemaps");
                await jsonTextWriter.WriteStartArrayAsync();
                foreach (var sitemap in sitemapSearchResult.Results)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    _jsonSerializer.Serialize(jsonTextWriter, sitemap);
                }
                await jsonTextWriter.WriteEndArrayAsync();

                cancellationToken.ThrowIfCancellationRequested();

                progressInfo.Description = "Sitemaps items loading...";
                progressCallback(progressInfo);

                // Load sitemap items
                var sitemapItemsSearchCriteria = new SitemapItemSearchCriteria
                {
                    Skip = 0,
                    Take = int.MaxValue
                };
                var sitemapItemsSearchResult = await _sitemapItemService.SearchAsync(sitemapItemsSearchCriteria);

                await jsonTextWriter.WritePropertyNameAsync("SitemapItems");
                await jsonTextWriter.WriteStartArrayAsync();
                foreach (var sitemapItem in sitemapItemsSearchResult.Results)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    _jsonSerializer.Serialize(jsonTextWriter, sitemapItem);
                }
                await jsonTextWriter.WriteEndArrayAsync();

                await jsonTextWriter.WriteEndObjectAsync();
            }
        }

        public async Task DoImportAsync(Stream backupStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            var progressInfo = new ExportImportProgressInfo();

            using (var textReader = new StreamReader(backupStream))
            using (var jsonTextReader = new JsonTextReader(textReader))
            {
                while (jsonTextReader.Read())
                {
                    if (jsonTextReader.TokenType == JsonToken.PropertyName)
                    {
                        if (jsonTextReader.Value.ToString() == "Sitemaps" &&
                            TryReadCollectionOf<Sitemap>(jsonTextReader, out var sitemaps))
                        {
                            progressInfo.Description = "Sitemaps importing...";
                            progressCallback(progressInfo);
                            await _sitemapService.SaveChangesAsync(sitemaps.ToArray());
                        }
                        else if (jsonTextReader.Value.ToString() == "SitemapItems" &&
                                 TryReadCollectionOf<SitemapItem>(jsonTextReader, out var sitemapItems))
                        {
                            progressInfo.Description = "Sitemaps items importing...";
                            progressCallback(progressInfo);
                            await _sitemapItemService.SaveChangesAsync(sitemapItems.ToArray());
                        }
                    }
                }
            }
        }

        private bool TryReadCollectionOf<TValue>(JsonReader jsonReader, out IReadOnlyCollection<TValue> values)
        {
            jsonReader.Read();
            if (jsonReader.TokenType == JsonToken.StartArray)
            {
                jsonReader.Read();

                var items = new List<TValue>();
                while (jsonReader.TokenType != JsonToken.EndArray)
                {
                    var item = _jsonSerializer.Deserialize<TValue>(jsonReader);
                    items.Add(item);

                    jsonReader.Read();
                }

                values = items;
                return true;
            }

            values = new TValue[0];
            return false;
        }
    }
}
