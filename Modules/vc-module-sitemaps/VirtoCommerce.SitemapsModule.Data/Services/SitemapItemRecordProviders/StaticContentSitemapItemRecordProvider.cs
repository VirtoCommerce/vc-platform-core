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

        public virtual async Task LoadSitemapItemRecordsAsync(Store store, Sitemap sitemap,
            string baseUrl, Action<ExportImportProgressInfo> progressCallback = null)
        {
            var contentBasePath = $"Pages/{sitemap.StoreId}";
            var storageProvider = _contentStorageProviderFactory(contentBasePath);
            var options = new SitemapItemOptions();
            var staticContentSitemapItems = GetStaticContentSitemapItems(sitemap);
            var totalCount = staticContentSitemapItems.Count;
            var progressInfo = GetProgressInfo(progressCallback, totalCount);
            var acceptedFilenameExtensions = GetAcceptedFilenameExtensions();

            progressInfo.Start();

            foreach (var sitemapItem in staticContentSitemapItems)
            {
                var urls = new List<string>();
                if (sitemapItem.ObjectType.EqualsInvariant(SitemapItemTypes.Folder))
                {
                    var folderUrls = await GetSiteMapFolderUrls(storageProvider, acceptedFilenameExtensions, sitemapItem);
                    urls.AddRange(folderUrls);
                }
                else if (sitemapItem.ObjectType.EqualsInvariant(SitemapItemTypes.ContentItem))
                {
                    var item = await storageProvider.GetBlobInfoAsync(sitemapItem.UrlTemplate);
                    if (item != null)
                    {
                        urls.Add(item.RelativeUrl);
                    }
                }

                var sitemapItemsRecords = GetSitemapItemsRecords(store, baseUrl, storageProvider, options, urls);
                sitemapItem.ItemsRecords.AddRange(sitemapItemsRecords);

                progressInfo.Next();
            }

            progressInfo.End();
        }

        private IEnumerable<SitemapItemRecord> GetSitemapItemsRecords(Store store, string baseUrl,
            IBlobContentStorageProvider storageProvider, SitemapItemOptions options, IEnumerable<string> urls)
        {
            var result = new List<SitemapItemRecord>();
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
                    result.AddRange(GetSitemapItemRecords(store, options, frontMatterPermalink.ToUrl().TrimStart('/'), baseUrl));
                }
            }

            return result;
        }

        private List<string> GetAcceptedFilenameExtensions()
        {
            return SettingsManager.GetValue("Sitemap.AcceptedFilenameExtensions", ".md,.html")
                .Split(',')
                .Select(i => i.Trim())
                .Where(i => !string.IsNullOrEmpty(i))
                .ToList();
        }

        private List<SitemapItem> GetStaticContentSitemapItems(Sitemap sitemap)
        {
            return sitemap.Items.Where(si => !string.IsNullOrEmpty(si.ObjectType) &&
                                            (si.ObjectType.EqualsInvariant(SitemapItemTypes.ContentItem) ||
                                            si.ObjectType.EqualsInvariant(SitemapItemTypes.Folder)))
                                 .ToList();
        }

        private async Task<IEnumerable<string>> GetSiteMapFolderUrls(IBlobContentStorageProvider storageProvider,
            ICollection<string> acceptedFilenameExtensions, SitemapItem sitemapItem)
        {
            var result = new List<string>();
            var searchResult = await storageProvider.SearchAsync(sitemapItem.UrlTemplate, null);
            var itemUrls = await GetItemUrls(storageProvider, searchResult);

            foreach (var itemUrl in itemUrls)
            {
                var itemExtension = Path.GetExtension(itemUrl);
                if (!acceptedFilenameExtensions.Any() ||
                    string.IsNullOrEmpty(itemExtension) ||
                    acceptedFilenameExtensions.Contains(itemExtension, StringComparer.OrdinalIgnoreCase))
                {
                    result.Add(itemUrl);
                }
            }

            return result;
        }

        private async Task<ICollection<string>> GetItemUrls(IBlobContentStorageProvider storageProvider, GenericSearchResult<BlobEntry> searchResult)
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

        private IDictionary<string, IEnumerable<string>> ReadYamlHeader(string text)
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

        private IEnumerable<string> GetYamlNodeValues(YamlNode value)
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

        private SitemapProgressInfo GetProgressInfo(Action<ExportImportProgressInfo> progressCallback, long totalCount)
        {
            return new SitemapProgressInfo
            {
                StartDescriptionTemplate = "Content: start generating records for {0} pages",
                EndDescriptionTemplate = "Content: {0} pages records generated",
                ProgressDescriptionTemplate = "Content: generated records for {0} of {1} pages",
                ProgressCallback = progressCallback,
                TotalCount = totalCount
            };
        }
    }
}
