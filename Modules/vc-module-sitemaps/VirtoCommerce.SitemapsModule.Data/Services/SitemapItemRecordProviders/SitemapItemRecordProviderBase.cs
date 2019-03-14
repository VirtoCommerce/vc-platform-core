using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.SitemapsModule.Data.Services.SitemapItemRecordProviders
{
    public abstract class SitemapItemRecordProviderBase
    {
        protected SitemapItemRecordProviderBase(ISettingsManager settingsManager, ISitemapUrlBuilder urlBuilider)
        {
            SettingsManager = settingsManager;
            UrlBuilder = urlBuilider;
        }

        protected ISettingsManager SettingsManager { get; }
        protected ISitemapUrlBuilder UrlBuilder { get; }

        public ICollection<SitemapItemRecord> GetSitemapItemRecords(Store store, SitemapItemOptions options, string urlTemplate, string baseUrl, IEntity entity = null)
        {
            var auditableEntity = entity as AuditableEntity;

            var result = new SitemapItemRecord
            {
                ModifiedDate = auditableEntity != null ? auditableEntity.ModifiedDate.Value : DateTime.UtcNow,
                Priority = options.Priority,
                UpdateFrequency = options.UpdateFrequency,
                Url = UrlBuilder.BuildStoreUrl(store, store.DefaultLanguage, urlTemplate, baseUrl, entity)
            };
            if (entity is ISeoSupport seoSupport)
            {
                foreach (var seoInfo in seoSupport.SeoInfos.Where(x => x.IsActive))
                {
                    if (store.Languages.Contains(seoInfo.LanguageCode) && !store.DefaultLanguage.EqualsInvariant(seoInfo.LanguageCode))
                    {
                        var alternate = new SitemapItemAlternateLinkRecord
                        {
                            Language = seoInfo.LanguageCode,
                            Type = "alternate",
                            Url = UrlBuilder.BuildStoreUrl(store, seoInfo.LanguageCode, urlTemplate, baseUrl, entity)
                        };
                        result.Alternates.Add(alternate);
                    }
                }
            }
            return new[] { result }.ToList();
        }       
    }
}
