using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Models.Xml;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.SitemapsModule.Data.Services
{
    public class SitemapXmlGenerator : ISitemapXmlGenerator
    {
        protected ILogger _logger { get; private set; }
        protected ISitemapService SitemapService { get; private set; }
        protected ISitemapItemService SitemapItemService { get; private set; }
        protected ISitemapUrlBuilder SitemapUrlBuilder { get; private set; }
        protected ISitemapItemRecordProvider[] SitemapItemRecordProviders { get; private set; }
        protected ISettingsManager SettingsManager { get; private set; }
        protected IStoreService StoreService { get; private set; }

        public SitemapXmlGenerator(ISitemapService sitemapService, ISitemapItemService sitemapItemService, ISitemapUrlBuilder sitemapUrlBuilder,
            ISitemapItemRecordProvider[] sitemapItemRecordProviders, ISettingsManager settingsManager, ILogger logger, IStoreService storeService)
        {
            SitemapService = sitemapService;
            SitemapItemService = sitemapItemService;
            SitemapUrlBuilder = sitemapUrlBuilder;
            SitemapItemRecordProviders = sitemapItemRecordProviders;
            SettingsManager = settingsManager;
            _logger = logger;
            StoreService = storeService;
        }


        public virtual async Task<ICollection<string>> GetSitemapUrlsAsync(string storeId)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                throw new ArgumentException("storeId");
            }

            var sitemapUrls = new List<string>();

            var store = await StoreService.GetByIdAsync(storeId);
            var sitemaps = await LoadAllStoreSitemapsAsync(store, "");
            foreach (var sitemap in sitemaps)
            {
                sitemapUrls.AddRange(sitemap.PagedLocations);
            }

            return sitemapUrls;
        }

        public virtual async Task<Stream> GenerateSitemapXmlAsync(string storeId, string baseUrl, string sitemapUrl, Action<ExportImportProgressInfo> progressCallback = null)
        {
            var stream = new MemoryStream();

            var filenameSeparator = SettingsManager.GetValue("Sitemap.FilenameSeparator", "--");
            var recordsLimitPerFile = SettingsManager.GetValue("Sitemap.RecordsLimitPerFile", 10000);

            XmlSerializer xmlSerializer = null;
            var xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add("", "http://www.sitemaps.org/schemas/sitemap/0.9");
            xmlNamespaces.Add("xhtml", "http://www.w3.org/1999/xhtml");

            var sitemapLocation = SitemapLocation.Parse(sitemapUrl, filenameSeparator);
            var store = await StoreService.GetByIdAsync(storeId);
            if (sitemapLocation.Location.EqualsInvariant("sitemap.xml"))
            {
                if (progressCallback != null)
                {
                    progressCallback(new ExportImportProgressInfo
                    {
                        Description = "Creating sitemap.xml..."
                    });
                }

                var allStoreSitemaps = await LoadAllStoreSitemapsAsync(store, baseUrl);
                var sitemapIndexXmlRecord = new SitemapIndexXmlRecord();
                foreach (var sitemap in allStoreSitemaps)
                {
                    var xmlSiteMapRecords = sitemap.PagedLocations.Select(location => new SitemapIndexItemXmlRecord
                    {
                        //ModifiedDate = sitemap.Items.Select(x => x.ModifiedDate).OrderByDescending(x => x).FirstOrDefault()?.ToString("yyyy-MM-dd"),
                        Url = SitemapUrlBuilder.BuildStoreUrl(store, store.DefaultLanguage, location, baseUrl)
                    }).ToList();
                    sitemapIndexXmlRecord.Sitemaps.AddRange(xmlSiteMapRecords);
                }
                xmlSerializer = new XmlSerializer(sitemapIndexXmlRecord.GetType());
                xmlSerializer.Serialize(stream, sitemapIndexXmlRecord, xmlNamespaces);
            }
            else
            {
                var sitemapSearchResult = await SitemapService.SearchAsync(new SitemapSearchCriteria { Location = sitemapLocation.Location, StoreId = storeId, Skip = 0, Take = 1 });
                var sitemap = sitemapSearchResult.Results.FirstOrDefault();
                if (sitemap != null)
                {
                    await LoadSitemapRecordsAsync(store, sitemap, baseUrl, progressCallback);
                    var distinctRecords = sitemap.Items.SelectMany(x => x.ItemsRecords).GroupBy(x => x.Url).Select(x => x.FirstOrDefault());
                    var sitemapItemRecords = distinctRecords.Skip((sitemapLocation.PageNumber - 1) * recordsLimitPerFile).Take(recordsLimitPerFile).ToArray();
                    var sitemapRecord = new SitemapXmlRecord
                    {
                        Items = sitemapItemRecords.Select(i => new SitemapItemXmlRecord().ToXmlModel(i)).ToList()
                    };
                    if (sitemapRecord.Items.Count > 0)
                    {
                        xmlSerializer = new XmlSerializer(sitemapRecord.GetType());
                        xmlSerializer.Serialize(stream, sitemapRecord, xmlNamespaces);
                    }
                }
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private async Task<ICollection<Sitemap>> LoadAllStoreSitemapsAsync(Store store, string baseUrl)
        {
            var result = new List<Sitemap>();
            var sitemapSearchCriteria = new SitemapSearchCriteria
            {
                StoreId = store.Id,
                Skip = 0,
                Take = int.MaxValue
            };
            var sitemapSearchResult = await SitemapService.SearchAsync(sitemapSearchCriteria);
            foreach (var sitemap in sitemapSearchResult.Results)
            {
                await LoadSitemapRecordsAsync(store, sitemap, baseUrl);
                result.Add(sitemap);
            }
            return result;
        }

        private async Task LoadSitemapRecordsAsync(Store store, Sitemap sitemap, string baseUrl, Action<ExportImportProgressInfo> progressCallback = null)
        {
            var recordsLimitPerFile = SettingsManager.GetValue("Sitemap.RecordsLimitPerFile", 10000);
            var filenameSeparator = SettingsManager.GetValue("Sitemap.FilenameSeparator", "--");

            var sitemapItemSearchCriteria = new SitemapItemSearchCriteria
            {
                SitemapId = sitemap.Id,
                Skip = 0,
                Take = int.MaxValue
            };

            sitemap.Items = (await SitemapItemService.SearchAsync(sitemapItemSearchCriteria)).Results;

            foreach (var recordProvider in SitemapItemRecordProviders)
            {
                //Log exceptions to prevent fail whole sitemap.xml generation
                try
                {
                    recordProvider.LoadSitemapItemRecordsAsync(store, sitemap, baseUrl, progressCallback);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
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
