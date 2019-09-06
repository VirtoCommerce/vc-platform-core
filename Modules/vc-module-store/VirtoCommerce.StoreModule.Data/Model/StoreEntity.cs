using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.StoreModule.Data.Model
{
    public class StoreEntity : AuditableEntity, IHasOuterId
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        [StringLength(256)]
        public string Url { get; set; }

        public int StoreState { get; set; }

        [StringLength(128)]
        public string TimeZone { get; set; }

        [StringLength(128)]
        public string Country { get; set; }

        [StringLength(128)]
        public string Region { get; set; }

        [StringLength(128)]
        public string DefaultLanguage { get; set; }

        [StringLength(64)]
        public string DefaultCurrency { get; set; }

        [StringLength(128)]
        [Required]
        public string Catalog { get; set; }

        public int CreditCardSavePolicy { get; set; }

        [StringLength(128)]
        public string SecureUrl { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        [StringLength(128)]
        public string AdminEmail { get; set; }

        public bool DisplayOutOfStock { get; set; }

        [StringLength(128)]
        public string FulfillmentCenterId { get; set; }
        [StringLength(128)]
        public string ReturnsFulfillmentCenterId { get; set; }

        [StringLength(128)]
        public string OuterId { get; set; }

        #region Navigation Properties

        public virtual ObservableCollection<StoreLanguageEntity> Languages { get; set; }
            = new NullCollection<StoreLanguageEntity>();

        public virtual ObservableCollection<StoreCurrencyEntity> Currencies { get; set; }
            = new NullCollection<StoreCurrencyEntity>();

        public virtual ObservableCollection<StoreTrustedGroupEntity> TrustedGroups { get; set; }
            = new NullCollection<StoreTrustedGroupEntity>();

        public virtual ObservableCollection<StoreFulfillmentCenterEntity> FulfillmentCenters { get; set; }
            = new NullCollection<StoreFulfillmentCenterEntity>();

        public virtual ObservableCollection<SeoInfoEntity> SeoInfos { get; set; }
            = new NullCollection<SeoInfoEntity>();

        public virtual ObservableCollection<StoreDynamicPropertyObjectValueEntity> DynamicPropertyObjectValues { get; set; }
            = new NullCollection<StoreDynamicPropertyObjectValueEntity>();

        #endregion

        public virtual Store ToModel(Store store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            store.Id = Id;
            store.CreatedBy = CreatedBy;
            store.CreatedDate = CreatedDate;
            store.ModifiedBy = ModifiedBy;
            store.ModifiedDate = ModifiedDate;
            store.OuterId = OuterId;

            store.AdminEmail = AdminEmail;
            store.Catalog = Catalog;
            store.Country = Country;
            store.DefaultCurrency = DefaultCurrency;
            store.DefaultLanguage = DefaultLanguage;
            store.Description = Description;
            store.DisplayOutOfStock = DisplayOutOfStock;
            store.Email = Email;
            store.Name = Name;
            store.Region = Region;
            store.SecureUrl = SecureUrl;
            store.TimeZone = TimeZone;
            store.Url = Url;
            store.MainFulfillmentCenterId = FulfillmentCenterId;
            store.MainReturnsFulfillmentCenterId = ReturnsFulfillmentCenterId;
            store.StoreState = EnumUtility.SafeParse(StoreState.ToString(), Core.Model.StoreState.Open);
            store.Languages = Languages.Select(x => x.LanguageCode).ToList();
            store.Currencies = Currencies.Select(x => x.CurrencyCode).ToList();
            store.TrustedGroups = TrustedGroups.Select(x => x.GroupName).ToList();
            store.AdditionalFulfillmentCenterIds = FulfillmentCenters.Where(x => x.Type == FulfillmentCenterType.Main).Select(x => x.FulfillmentCenterId).ToList();
            store.ReturnsFulfillmentCenterIds = FulfillmentCenters.Where(x => x.Type == FulfillmentCenterType.Returns).Select(x => x.FulfillmentCenterId).ToList();
            store.SeoInfos = SeoInfos.Select(x => x.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance())).ToList();

            store.DynamicProperties = DynamicPropertyObjectValues.GroupBy(g => g.PropertyId).Select(x =>
            {
                var property = AbstractTypeFactory<DynamicObjectProperty>.TryCreateInstance();
                property.Id = x.Key;
                property.Name = x.FirstOrDefault()?.PropertyName;
                property.Values = x.Select(v => v.ToModel(AbstractTypeFactory<DynamicPropertyObjectValue>.TryCreateInstance())).ToArray();
                return property;
            }).ToArray();

            return store;
        }

        public virtual StoreEntity FromModel(Store store, PrimaryKeyResolvingMap pkMap)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            pkMap.AddPair(store, this);

            Id = store.Id;
            CreatedBy = store.CreatedBy;
            CreatedDate = store.CreatedDate;
            ModifiedBy = store.ModifiedBy;
            ModifiedDate = store.ModifiedDate;
            OuterId = store.OuterId;

            AdminEmail = store.AdminEmail;
            Catalog = store.Catalog;
            Country = store.Country;
            DefaultCurrency = store.DefaultCurrency;
            DefaultLanguage = store.DefaultLanguage;
            Description = store.Description;
            DisplayOutOfStock = store.DisplayOutOfStock;
            Email = store.Email;
            Name = store.Name;
            Region = store.Region;
            SecureUrl = store.SecureUrl;
            TimeZone = store.TimeZone;
            Url = store.Url;
            StoreState = (int)store.StoreState;

            if (store.DefaultCurrency != null)
            {
                DefaultCurrency = store.DefaultCurrency;
            }

            if (store.MainFulfillmentCenterId != null)
            {
                FulfillmentCenterId = store.MainFulfillmentCenterId;
            }

            if (store.MainReturnsFulfillmentCenterId != null)
            {
                ReturnsFulfillmentCenterId = store.MainReturnsFulfillmentCenterId;
            }

            if (store.Languages != null)
            {
                Languages = new ObservableCollection<StoreLanguageEntity>(store.Languages.Select(x => new StoreLanguageEntity
                {
                    LanguageCode = x
                }));
            }

            if (store.Currencies != null)
            {
                Currencies = new ObservableCollection<StoreCurrencyEntity>(store.Currencies.Select(x => new StoreCurrencyEntity
                {
                    CurrencyCode = x.ToString()
                }));
            }

            if (store.TrustedGroups != null)
            {
                TrustedGroups = new ObservableCollection<StoreTrustedGroupEntity>(store.TrustedGroups.Select(x => new StoreTrustedGroupEntity
                {
                    GroupName = x
                }));
            }

            FulfillmentCenters = new ObservableCollection<StoreFulfillmentCenterEntity>();
            if (store.AdditionalFulfillmentCenterIds != null)
            {

                FulfillmentCenters.AddRange(store.AdditionalFulfillmentCenterIds.Select(fc => new StoreFulfillmentCenterEntity
                {
                    FulfillmentCenterId = fc,
                    Name = fc,
                    StoreId = store.Id,
                    Type = FulfillmentCenterType.Main
                }));
            }
            if (store.ReturnsFulfillmentCenterIds != null)
            {
                FulfillmentCenters.AddRange(store.ReturnsFulfillmentCenterIds.Select(fc => new StoreFulfillmentCenterEntity
                {
                    FulfillmentCenterId = fc,
                    Name = fc,
                    StoreId = store.Id,
                    Type = FulfillmentCenterType.Returns
                }));
            }

            if (store.SeoInfos != null)
            {
                SeoInfos = new ObservableCollection<SeoInfoEntity>(store.SeoInfos.Select(x => AbstractTypeFactory<SeoInfoEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }

            if (store.DynamicProperties != null)
            {
                DynamicPropertyObjectValues = new ObservableCollection<StoreDynamicPropertyObjectValueEntity>(store.DynamicProperties.SelectMany(p => p.Values
                    .Select(v => AbstractTypeFactory<StoreDynamicPropertyObjectValueEntity>.TryCreateInstance().FromModel(v, store, p))).OfType<StoreDynamicPropertyObjectValueEntity>());
            }

            return this;
        }

        public virtual void Patch(StoreEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.AdminEmail = AdminEmail;
            target.Catalog = Catalog;
            target.Country = Country;
            target.DefaultCurrency = DefaultCurrency;
            target.DefaultLanguage = DefaultLanguage;
            target.Description = Description;
            target.DisplayOutOfStock = DisplayOutOfStock;
            target.Email = Email;
            target.ModifiedBy = ModifiedBy;
            target.ModifiedDate = ModifiedDate;
            target.Name = Name;
            target.Region = Region;
            target.SecureUrl = SecureUrl;
            target.TimeZone = TimeZone;
            target.Url = Url;
            target.StoreState = StoreState;
            target.FulfillmentCenterId = FulfillmentCenterId;
            target.ReturnsFulfillmentCenterId = ReturnsFulfillmentCenterId;

            if (!Languages.IsNullCollection())
            {
                var languageComparer = AnonymousComparer.Create((StoreLanguageEntity x) => x.LanguageCode);
                Languages.Patch(target.Languages, languageComparer,
                                      (sourceLang, targetLang) => targetLang.LanguageCode = sourceLang.LanguageCode);
            }

            if (!Currencies.IsNullCollection())
            {
                var currencyComparer = AnonymousComparer.Create((StoreCurrencyEntity x) => x.CurrencyCode);
                Currencies.Patch(target.Currencies, currencyComparer,
                                      (sourceCurrency, targetCurrency) => targetCurrency.CurrencyCode = sourceCurrency.CurrencyCode);
            }

            if (!TrustedGroups.IsNullCollection())
            {
                var trustedGroupComparer = AnonymousComparer.Create((StoreTrustedGroupEntity x) => x.GroupName);
                TrustedGroups.Patch(target.TrustedGroups, trustedGroupComparer,
                                      (sourceGroup, targetGroup) => sourceGroup.GroupName = targetGroup.GroupName);
            }

            if (!FulfillmentCenters.IsNullCollection())
            {
                var fulfillmentCenterComparer = AnonymousComparer.Create((StoreFulfillmentCenterEntity fc) => $"{fc.FulfillmentCenterId}-{fc.Type}");
                FulfillmentCenters.Patch(target.FulfillmentCenters, fulfillmentCenterComparer,
                                      (sourceFulfillmentCenter, targetFulfillmentCenter) => sourceFulfillmentCenter.Patch(targetFulfillmentCenter));
            }

            if (!SeoInfos.IsNullCollection())
            {
                SeoInfos.Patch(target.SeoInfos, (sourceSeoInfo, targetSeoInfo) => sourceSeoInfo.Patch(targetSeoInfo));
            }

            if (!DynamicPropertyObjectValues.IsNullCollection())
            {
                DynamicPropertyObjectValues.Patch(target.DynamicPropertyObjectValues, (sourceDynamicPropertyObjectValues, targetDynamicPropertyObjectValues) => sourceDynamicPropertyObjectValues.Patch(targetDynamicPropertyObjectValues));
            }
        }
    }
}
