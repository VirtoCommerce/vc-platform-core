using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SitemapsModule.Core;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Models.Search;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Models.Xml;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.SitemapsModule.Data.Services
{
    public class SitemapXmlGenerator : ISitemapXmlGenerator
    {
        private readonly ILogger _logger;
        private readonly ISitemapSearchService _sitemapSearchService;
        private readonly ISitemapItemSearchService _sitemapItemSearchService;
        private readonly ISitemapUrlBuilder _sitemapUrlBuilder;
        private readonly IEnumerable<ISitemapItemRecordProvider> _sitemapItemRecordProviders;
        private readonly ISettingsManager _settingsManager;
        private readonly IStoreService _storeService;


        public SitemapXmlGenerator(
            ISitemapSearchService sitemapSearchService,
            ISitemapItemSearchService sitemapItemSearchService,
            ISitemapUrlBuilder sitemapUrlBuilder,
            IEnumerable<ISitemapItemRecordProvider> sitemapItemRecordProviders,
            ISettingsManager settingsManager,
            ILogger<SitemapXmlGenerator> logger,
            IStoreService storeService)
        {
            _sitemapSearchService = sitemapSearchService;
            _sitemapItemSearchService = sitemapItemSearchService;
            _sitemapUrlBuilder = sitemapUrlBuilder;
            _sitemapItemRecordProviders = sitemapItemRecordProviders;
            _settingsManager = settingsManager;
            _logger = logger;
            _storeService = storeService;
        }


        public virtual async Task<ICollection<string>> GetSitemapUrlsAsync(string storeId)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                throw new ArgumentException(nameof(storeId));
            }

            var sitemapUrls = new List<string>();
            var store = await _storeService.GetByIdAsync(storeId, StoreResponseGroup.StoreInfo.ToString());

            var sitemapSearchCriteria = new SitemapSearchCriteria
            {
                StoreId = store.Id,
                Skip = 0,
                Take = int.MaxValue
            };

            var sitemaps = await LoadAllStoreSitemaps(store, "");
            foreach (var sitemap in sitemaps)
            {
                sitemapUrls.AddRange(sitemap.PagedLocations);
            }

            return sitemapUrls;
        }

        public virtual async Task<Stream> GenerateSitemapXmlAsync(string storeId, string baseUrl, string sitemapUrl, Action<ExportImportProgressInfo> progressCallback = null)
        {
            var stream = new MemoryStream();

            var filenameSeparator = _settingsManager.GetValue(ModuleConstants.Settings.General.FilenameSeparator.Name, "--");
            var recordsLimitPerFile = _settingsManager.GetValue(ModuleConstants.Settings.General.RecordsLimitPerFile.Name, 10000);

            var xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add("", "http://www.sitemaps.org/schemas/sitemap/0.9");
            xmlNamespaces.Add("xhtml", "http://www.w3.org/1999/xhtml");

            var sitemapLocation = SitemapLocation.Parse(sitemapUrl, filenameSeparator);
            var store = await _storeService.GetByIdAsync(storeId, StoreResponseGroup.StoreInfo.ToString());
            if (sitemapLocation.Location.EqualsInvariant("sitemap.xml"))
            {
                progressCallback?.Invoke(new ExportImportProgressInfo
                {
                    Description = "Creating sitemap.xml..."
                });

                var allStoreSitemaps = await LoadAllStoreSitemaps(store, baseUrl);
                var sitemapIndexXmlRecord = new SitemapIndexXmlRecord();
                foreach (var sitemap in allStoreSitemaps)
                {
                    var xmlSiteMapRecords = sitemap.PagedLocations.Select(location => new SitemapIndexItemXmlRecord
                    {
                        //ModifiedDate = sitemap.Items.Select(x => x.ModifiedDate).OrderByDescending(x => x).FirstOrDefault()?.ToString("yyyy-MM-dd"),
                        Url = _sitemapUrlBuilder.BuildStoreUrl(store, store.DefaultLanguage, location, baseUrl)
                    }).ToList();
                    sitemapIndexXmlRecord.Sitemaps.AddRange(xmlSiteMapRecords);
                }
                var xmlSerializer = new XmlSerializer(sitemapIndexXmlRecord.GetType());
                xmlSerializer.Serialize(stream, sitemapIndexXmlRecord, xmlNamespaces);
            }
            else
            {
                var sitemapSearchResult = await _sitemapSearchService.SearchAsync(new SitemapSearchCriteria { Location = sitemapLocation.Location, StoreId = storeId, Skip = 0, Take = 1 });
                var sitemap = sitemapSearchResult.Results.FirstOrDefault();
                if (sitemap != null)
                {
                    await LoadSitemapRecords(store, sitemap, baseUrl, progressCallback);
                    var distinctRecords = sitemap.Items.SelectMany(x => x.ItemsRecords).GroupBy(x => x.Url).Select(x => x.FirstOrDefault());
                    var sitemapItemRecords = distinctRecords.Skip((sitemapLocation.PageNumber - 1) * recordsLimitPerFile).Take(recordsLimitPerFile).ToArray();
                    var sitemapRecord = new SitemapXmlRecord
                    {
                        Items = sitemapItemRecords.Select(i => new SitemapItemXmlRecord().ToXmlModel(i)).ToList()
                    };
                    if (sitemapRecord.Items.Count > 0)
                    {
                        var xmlSerializer = new XmlSerializer(sitemapRecord.GetType());
                        xmlSerializer.Serialize(stream, sitemapRecord, xmlNamespaces);
                    }
                }
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private async Task<ICollection<Sitemap>> LoadAllStoreSitemaps(Store store, string baseUrl)
        {
            var result = new List<Sitemap>();
            var sitemapSearchCriteria = new SitemapSearchCriteria
            {
                StoreId = store.Id,
                Skip = 0,
                Take = int.MaxValue
            };
            var sitemapSearchResult = await _sitemapSearchService.SearchAsync(sitemapSearchCriteria);
            foreach (var sitemap in sitemapSearchResult.Results)
            {
                await LoadSitemapRecords(store, sitemap, baseUrl);
                result.Add(sitemap);
            }
            return result;
        }

        private async Task LoadSitemapRecords(Store store, Sitemap sitemap, string baseUrl, Action<ExportImportProgressInfo> progressCallback = null)
        {
            var recordsLimitPerFile = _settingsManager.GetValue(ModuleConstants.Settings.General.RecordsLimitPerFile.Name, 10000);
            var filenameSeparator = _settingsManager.GetValue(ModuleConstants.Settings.General.FilenameSeparator.Name, "--");

            var sitemapItemSearchCriteria = new SitemapItemSearchCriteria
            {
                SitemapId = sitemap.Id,
                Skip = 0,
                Take = int.MaxValue
            };
            sitemap.Items = (await _sitemapItemSearchService.SearchAsync(sitemapItemSearchCriteria)).Results;
            foreach (var recordProvider in _sitemapItemRecordProviders)
            {
                //Log exceptions to prevent fail whole sitemap.xml generation
                try
                {
                    await recordProvider.LoadSitemapItemRecordsAsync(store, sitemap, baseUrl, progressCallback);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to load sitemap item records for store #{store.Id}, sitemap #{sitemap.Id} and baseURL '{baseUrl}'");
                }
            }
            sitemap.PagedLocations.Clear();
            var totalRecordsCount = sitemap.Items.SelectMany(x => x.ItemsRecords).GroupBy(x => x.Url).Count();
            var pagesCount = totalRecordsCount > 0 ? (int)Math.Ceiling(totalRecordsCount / (double)recordsLimitPerFile) : 0;
            for (var i = 1; i <= pagesCount; i++)
            {
                sitemap.PagedLocations.Add(new SitemapLocation(sitemap.Location, i, filenameSeparator).ToString(pagesCount > 1));
            }
        }
    }
}
