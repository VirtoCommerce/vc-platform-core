using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core;
using VirtoCommerce.MarketingModule.Core.Events;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.ExportImport;
using VirtoCommerce.MarketingModule.Data.Handlers;
using VirtoCommerce.MarketingModule.Data.Promotions;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.MarketingModule.Data.Services;
using VirtoCommerce.MarketingModule.Web.ExportImport;
using VirtoCommerce.MarketingModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.MarketingModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        private IApplicationBuilder _appBuilder;
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Marketing") ?? configuration.GetConnectionString("VirtoCommerce");

            serviceCollection.AddTransient<IMarketingRepository, MarketingRepositoryImpl>();
            serviceCollection.AddDbContext<MarketingDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<Func<IMarketingRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IMarketingRepository>());

            var promotionExtensionManager = new DefaultMarketingExtensionManagerImpl();

            serviceCollection.AddSingleton<IMarketingExtensionManager>(promotionExtensionManager);
            serviceCollection.AddSingleton<IPromotionService, PromotionServiceImpl>();
            serviceCollection.AddSingleton<ICouponService, CouponService>();
            serviceCollection.AddSingleton<IPromotionUsageService, PromotionUsageService>();
            serviceCollection.AddSingleton<IMarketingDynamicContentEvaluator, DefaultDynamicContentEvaluatorImpl>();
            serviceCollection.AddSingleton<IDynamicContentService, DynamicContentServiceImpl>();

            serviceCollection.AddSingleton<IPromotionSearchService, MarketingSearchServiceImpl>();
            serviceCollection.AddSingleton<ICouponService, CouponService>();
            serviceCollection.AddSingleton<IDynamicContentSearchService, MarketingSearchServiceImpl>();
            serviceCollection.AddSingleton<CsvCouponImporter>();

            //var settingsManager = serviceCollection.BuildServiceProvider().GetRequiredService<ISettingsManager>();
            //var promotionCombinePolicy = settingsManager.GetValue("Marketing.Promotion.CombinePolicy", "BestReward");
            //if (promotionCombinePolicy.EqualsInvariant("CombineStackable"))
            //{
            //    serviceCollection.AddSingleton<IMarketingPromoEvaluator, CombineStackablePromotionPolicy>();
            //}
            //else
            //{
            serviceCollection.AddSingleton<IMarketingPromoEvaluator, BestRewardPromotionPolicy>();
            //}

            AbstractTypeFactory<DynamicPromotion>.RegisterType<DynamicPromotion>().WithFactory(() =>
            {
                var couponService = serviceCollection.BuildServiceProvider().GetService<ICouponService>();
                var promotionUsageService = serviceCollection.BuildServiceProvider().GetService<IPromotionUsageService>();
                return new DynamicPromotion(couponService, promotionUsageService);
            });
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.General.AllSettings, ModuleInfo.Id);

            var settingsManager = appBuilder.ApplicationServices.GetRequiredService<ISettingsManager>();
            var promotionCombinePolicy = settingsManager.GetValue("Marketing.Promotion.CombinePolicy", "BestReward");

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Marketing",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            var eventHandlerRegistrar = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            //Create order observer. record order coupon usage
            eventHandlerRegistrar.RegisterHandler<DynamicContentItemChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<DynamicContentItemEventHandlers>().Handle(message));

            var dynamicContentService = appBuilder.ApplicationServices.GetService<IDynamicContentService>();
            foreach (var id in new[] { Model.MarketingConstants.ContentPlacesRootFolderId, Model.MarketingConstants.CotentItemRootFolderId })
            {
                var folders = dynamicContentService.GetFoldersByIdsAsync(new[] { id }).GetAwaiter().GetResult();
                var rootFolder = folders.FirstOrDefault();
                if (rootFolder == null)
                {
                    rootFolder = new DynamicContentFolder
                    {
                        Id = id,
                        Name = id
                    };
                    dynamicContentService.SaveFoldersAsync(new[] { rootFolder }).GetAwaiter().GetResult();
                }
            }

            //Create standard dynamic properties for dynamic content item
            var dynamicPropertyService = appBuilder.ApplicationServices.GetService<IDynamicPropertyService>();
            var contentItemTypeProperty = new DynamicProperty
            {
                Id = "Marketing_DynamicContentItem_Type_Property",
                IsDictionary = true,
                Name = "Content type",
                ObjectType = typeof(DynamicContentItem).FullName,
                ValueType = DynamicPropertyValueType.ShortText,
                CreatedBy = "Auto",
            };

            dynamicPropertyService.SaveDynamicPropertiesAsync(new[] { contentItemTypeProperty }).GetAwaiter().GetResult();

            var promotionExtensionManager = appBuilder.ApplicationServices.GetService<IMarketingExtensionManager>();
            promotionExtensionManager.PromotionCondition = new PromotionConditionReward()
            {
                Children = GetConditionsAndRewards(),
            };

            //Next lines allow to use polymorph types in API controller methods
            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicMarketingJsonConverter(appBuilder.ApplicationServices.GetService<IMarketingExtensionManager>()));
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PromotionConditionRewardJsonConverter());

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<MarketingDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

            AbstractTypeFactory<IConditionRewardTree>.RegisterType<PromotionConditionReward>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<BlockConditionAndOr>();

            AbstractTypeFactory<IConditionRewardTree>.RegisterType<BlockCustomerCondition>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionIsRegisteredUser>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionIsEveryone>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionIsFirstTimeBuyer>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<UserGroupsContainsCondition>();

            AbstractTypeFactory<IConditionRewardTree>.RegisterType<BlockCatalogCondition>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionAtNumItemsInCart>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionAtNumItemsInCategoryAreInCart>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionAtNumItemsOfEntryAreInCart>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionCartSubtotalLeast>();

            AbstractTypeFactory<IConditionRewardTree>.RegisterType<BlockCartCondition>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionCategoryIs>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionCodeContains>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionCurrencyIs>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionEntryIs>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<ConditionInStockQuantity>();

            AbstractTypeFactory<IConditionRewardTree>.RegisterType<BlockReward>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardCartGetOfAbsSubtotal>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardCartGetOfRelSubtotal>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemGetFreeNumItemOfProduct>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemGetOfAbs>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemGetOfAbsForNum>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemGetOfRel>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemGetOfRelForNum>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemGiftNumItem>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardShippingGetOfAbsShippingMethod>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardShippingGetOfRelShippingMethod>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardPaymentGetOfAbs>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardPaymentGetOfRel>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemForEveryNumInGetOfRel>();
            AbstractTypeFactory<IConditionRewardTree>.RegisterType<RewardItemForEveryNumOtherItemInGetOfRel>();

            //AbstractTypeFactory<PromotionReward>.RegisterType<AmountBasedReward>();
            AbstractTypeFactory<PromotionReward>.RegisterType<GiftReward>();
            AbstractTypeFactory<PromotionReward>.RegisterType<CartSubtotalReward>();
            AbstractTypeFactory<PromotionReward>.RegisterType<CatalogItemAmountReward>();
            AbstractTypeFactory<PromotionReward>.RegisterType<PaymentReward>();
            AbstractTypeFactory<PromotionReward>.RegisterType<ShipmentReward>();
            AbstractTypeFactory<PromotionReward>.RegisterType<SpecialOfferReward>();
        }

        public void Uninstall()
        {
        }

        public Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<MarketingExportImport>().ExportAsync(outStream, options, progressCallback, cancellationToken);
        }

        public Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<MarketingExportImport>().ImportAsync(inputStream, options, progressCallback, cancellationToken);
        }

        private List<IConditionRewardTree> GetConditionsAndRewards()
        {
            return new List<IConditionRewardTree>
            {
                new BlockCustomerCondition
                {
                    AvailableChildren = new List<IConditionRewardTree>
                    {
                        new ConditionIsRegisteredUser(), new ConditionIsEveryone(),
                        new ConditionIsFirstTimeBuyer(), new UserGroupsContainsCondition()
                    }
                },
                new BlockCatalogCondition
                {
                    AvailableChildren = new List<IConditionRewardTree>
                    {
                        new ConditionCategoryIs(), new ConditionCodeContains(), new ConditionCurrencyIs(),
                        new ConditionEntryIs(), new ConditionInStockQuantity()
                    }
                },
                new BlockCartCondition
                {
                    AvailableChildren = new List<IConditionRewardTree>
                    {
                        new ConditionAtNumItemsInCart(), new ConditionAtNumItemsInCategoryAreInCart(),
                        new ConditionAtNumItemsOfEntryAreInCart(), new ConditionCartSubtotalLeast()
                    }
                },
                new BlockReward
                {
                    AvailableChildren = new List<IConditionRewardTree>
                    {
                        new RewardCartGetOfAbsSubtotal(), new RewardCartGetOfRelSubtotal(), new RewardItemGetFreeNumItemOfProduct(), new RewardItemGetOfAbs(),
                        new RewardItemGetOfAbsForNum(), new RewardItemGetOfRel(), new RewardItemGetOfRelForNum(),
                        new RewardItemGiftNumItem(), new RewardShippingGetOfAbsShippingMethod(), new RewardShippingGetOfRelShippingMethod(), new RewardPaymentGetOfAbs(),
                        new RewardPaymentGetOfRel(), new RewardItemForEveryNumInGetOfRel(), new RewardItemForEveryNumOtherItemInGetOfRel()
                    }
                }
            };
        }
    }
}
