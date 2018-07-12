using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.SitemapsModule.Data.Services.SitemapItemRecordProviders
{
    public abstract class SitemapItemRecordProviderBase
    {
        protected ISettingsManager _settingsManager { get; private set; }
        protected ISitemapUrlBuilder _urlBuilder { get; private set; }

        public SitemapItemRecordProviderBase(ISettingsManager settingsManager, ISitemapUrlBuilder urlBuilider)
        {
            _settingsManager = settingsManager;
            _urlBuilder = urlBuilider;
        }
        
        public ICollection<SitemapItemRecord> GetSitemapItemRecords(Store store, SitemapItemOptions options, string urlTemplate, string baseUrl, IEntity entity = null)
        {
            var auditableEntity = entity as AuditableEntity;

            var result = new SitemapItemRecord
            {
                ModifiedDate = auditableEntity != null ? auditableEntity.ModifiedDate.Value : DateTime.UtcNow,
                Priority = options.Priority,
                UpdateFrequency = options.UpdateFrequency,
                Url = _urlBuilder.BuildStoreUrl(store, store.DefaultLanguage, urlTemplate, baseUrl, entity)
            };

            var seoSupport = entity as ISeoSupport;

            if (seoSupport != null)
            {
                foreach (var seoInfo in seoSupport.SeoInfos.Where(x => x.IsActive))
                {
                    if (store.Languages.Contains(seoInfo.LanguageCode) && !store.DefaultLanguage.EqualsInvariant(seoInfo.LanguageCode))
                    {
                        var alternate = new SitemapItemAlternateLinkRecord
                        {
                            Language = seoInfo.LanguageCode,
                            Type = "alternate",
                            Url = _urlBuilder.BuildStoreUrl(store, seoInfo.LanguageCode, urlTemplate, baseUrl, entity)
                        };
                        result.Alternates.Add(alternate);
                    }
                }
            }

            return new[] { result }.ToList();
        }
    }
}
