using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.SitemapsModule.Data.Services.SitemapItemRecordProviders
{
    public class StaticContentSitemapItemRecordProvider : SitemapItemRecordProviderBase, ISitemapItemRecordProvider
    {
        public StaticContentSitemapItemRecordProvider(
            ISitemapUrlBuilder urlBuilder,
            ISettingsManager settingsManager,
            Func<string, IContentBlobStorageProvider> contentStorageProviderFactory)
            : base(settingsManager, urlBuilder)
        {
            _contentStorageProviderFactory = contentStorageProviderFactory;
        }

        private readonly Func<string, IContentBlobStorageProvider> _contentStorageProviderFactory;
        private static readonly Regex _headerRegExp = new Regex(@"(?s:^---(.*?)---)");

        public virtual void LoadSitemapItemRecords(Store store, Sitemap sitemap, string baseUrl, Action<ExportImportProgressInfo> progressCallback = null)
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
                    var searchResult = storageProvider.Search(sitemapItem.UrlTemplate, null);
                    var itemUrls = GetItemUrls(storageProvider, searchResult);
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
                    var item = storageProvider.GetBlobInfo(sitemapItem.UrlTemplate);
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
                        IEnumerable<string> permalinks;
                        yamlHeader.TryGetValue("permalink", out permalinks);
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

        private static ICollection<string> GetItemUrls(IContentBlobStorageProvider storageProvider, BlobSearchResult searchResult)
        {
            var urls = new List<string>();

            foreach (var item in searchResult.Items)
            {
                urls.Add(item.RelativeUrl);
            }
            foreach (var folder in searchResult.Folders)
            {
                var folderSearchResult = storageProvider.Search(folder.RelativeUrl, null);
                urls.AddRange(GetItemUrls(storageProvider, folderSearchResult));
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
                var collection = root as YamlMappingNode;
                if (collection != null)
                {
                    foreach (var entry in collection.Children)
                    {
                        var node = entry.Key as YamlScalarNode;
                        if (node != null)
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
            var list = value as YamlSequenceNode;

            if (list != null)
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
