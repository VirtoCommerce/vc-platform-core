using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.Tools;
using YamlDotNet.RepresentationModel;

namespace VirtoCommerce.SitemapsModule.Data.Services.SitemapItemRecordProviders
{
    public class StaticContentSitemapItemRecordProvider : SitemapItemRecordProviderBase, ISitemapItemRecordProvider
    {
        public StaticContentSitemapItemRecordProvider(ISettingsManager settingsManager, ISitemapUrlBuilder urlBuilider)
            : base(settingsManager, urlBuilider)
        {
        }

        private readonly Func<string, IBlobContentStorageProvider> _contentStorageProviderFactory;
        private static readonly Regex _headerRegExp = new Regex(@"(?s:^---(.*?)---)");

        public StaticContentSitemapItemRecordProvider(
            ISitemapUrlBuilder urlBuilder,
            ISettingsManager settingsManager,
            Func<string, IBlobContentStorageProvider> contentStorageProviderFactory)
            : base(settingsManager, urlBuilder)
        {
            _contentStorageProviderFactory = contentStorageProviderFactory;
        }

        public virtual async Task LoadSitemapItemRecordsAsync(Store store, Sitemap sitemap, string baseUrl, Action<ExportImportProgressInfo> progressCallback = null)
        {
            var progressInfo = new ExportImportProgressInfo();

            var contentBasePath = $"Pages/{sitemap.StoreId}";
            var storageProvider = _contentStorageProviderFactory(contentBasePath);
            var options = new SitemapItemOptions();
            var staticContentSitemapItems = sitemap.Items.Where(si => !string.IsNullOrEmpty(si.ObjectType) &&
                                                                      (si.ObjectType.EqualsInvariant(SitemapItemTypes.ContentItem) ||
                                                                       si.ObjectType.EqualsInvariant(SitemapItemTypes.Folder)))
                                                                       .ToList();
            var totalCount = staticContentSitemapItems.Count;
            var processedCount = 0;

            var acceptedFilenameExtensions = SettingsManager.GetValue("Sitemap.AcceptedFilenameExtensions", ".md,.html")
                .Split(',')
                .Select(i => i.Trim())
                .Where(i => !string.IsNullOrEmpty(i))
                .ToList();

            progressInfo.Description = $"Content: start generating records for {totalCount} pages";
            progressCallback?.Invoke(progressInfo);

            foreach (var sitemapItem in staticContentSitemapItems)
            {
                var urls = new List<string>();
                if (sitemapItem.ObjectType.EqualsInvariant(SitemapItemTypes.Folder))
                {
                    var searchResult = await storageProvider.SearchAsync(sitemapItem.UrlTemplate, null);
                    var itemUrls = await GetItemUrls(storageProvider, searchResult);
                    foreach (var itemUrl in itemUrls)
                    {
                        var itemExtension = Path.GetExtension(itemUrl);
                        if (!acceptedFilenameExtensions.Any() ||
                            string.IsNullOrEmpty(itemExtension) ||
                            acceptedFilenameExtensions.Contains(itemExtension, StringComparer.OrdinalIgnoreCase))
                        {
                            urls.Add(itemUrl);
                        }
                    }
                }
                else if (sitemapItem.ObjectType.EqualsInvariant(SitemapItemTypes.ContentItem))
                {
                    var item = await storageProvider.GetBlobInfoAsync(sitemapItem.UrlTemplate);
                    if (item != null)
                    {
                        urls.Add(item.RelativeUrl);
                    }
                }
                totalCount = urls.Count;

                foreach (var url in urls)
                {
                    using (var stream = storageProvider.OpenRead(url))
                    {
                        var content = stream.ReadToString();
                        var yamlHeader = ReadYamlHeader(content);
                        yamlHeader.TryGetValue("permalink", out var permalinks);
                        var frontMatterPermalink = new FrontMatterPermalink(url.Replace(".md", ""));
                        if (permalinks != null)
                        {
                            frontMatterPermalink = new FrontMatterPermalink(permalinks.FirstOrDefault());
                        }
                        sitemapItem.ItemsRecords.AddRange(GetSitemapItemRecords(store, options, frontMatterPermalink.ToUrl().TrimStart('/'), baseUrl));

                        processedCount++;
                        progressInfo.Description = $"Content: generated records for {processedCount} of {totalCount} pages";
                        progressCallback?.Invoke(progressInfo);
                    }
                }
            }
        }

        private static async Task<ICollection<string>> GetItemUrls(IBlobContentStorageProvider storageProvider, GenericSearchResult<BlobEntry> searchResult)
        {
            var urls = new List<string>();

            foreach (var item in searchResult.Results.Where(x => x.Type.EqualsInvariant("blob")))
            {
                urls.Add(item.RelativeUrl);
            }

            foreach (var folder in searchResult.Results.Where(x => x.Type.EqualsInvariant("folder")))
            {
                var folderSearchResult = await storageProvider.SearchAsync(folder.RelativeUrl, null);
                urls.AddRange(await GetItemUrls(storageProvider, folderSearchResult));
            }

            return urls;
        }

        private static IDictionary<string, IEnumerable<string>> ReadYamlHeader(string text)
        {
            var retVal = new Dictionary<string, IEnumerable<string>>();
            var headerMatches = _headerRegExp.Matches(text);
            if (headerMatches.Count == 0)
                return retVal;

            var input = new StringReader(headerMatches[0].Groups[1].Value);
            var yaml = new YamlStream();

            yaml.Load(input);

            if (yaml.Documents.Count > 0)
            {
                var root = yaml.Documents[0].RootNode;
                if (root is YamlMappingNode collection)
                {
                    foreach (var entry in collection.Children)
                    {
                        if (entry.Key is YamlScalarNode node)
                        {
                            retVal.Add(node.Value, GetYamlNodeValues(entry.Value));
                        }
                    }
                }
            }
            return retVal;
        }

        private static IEnumerable<string> GetYamlNodeValues(YamlNode value)
        {
            var retVal = new List<string>();

            if (value is YamlSequenceNode list)
            {
                retVal.AddRange(list.Children.OfType<YamlScalarNode>().Select(node => node.Value));
            }
            else
            {
                retVal.Add(value.ToString());
            }

            return retVal;
        }
    }
}
