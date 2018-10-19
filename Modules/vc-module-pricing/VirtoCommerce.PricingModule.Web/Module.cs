using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Serialization;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Repositories;
using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions;
using VirtoCommerce.PricingModule.Core.ModuleConstants;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Common.Conditions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Common.Conditions.Browse;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Common.Conditions.GeoConditions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Content;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Pricing;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Conditions.CartConditions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Conditions.CatalogConditions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Conditions.CustomerConditions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Rewards;
using VirtoCommerce.PricingModule.Data.Model;
using VirtoCommerce.PricingModule.Data.Repositories;
using VirtoCommerce.PricingModule.Data.Search;
using VirtoCommerce.PricingModule.Data.Services;
using VirtoCommerce.PricingModule.Web.ExportImport;
using VirtoCommerce.PricingModule.Web.JsonConverters;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.PricingModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        private IApplicationBuilder _applicationBuilder;

        #region IModule Members

        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Pricing") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<PricingDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddTransient<IPricingRepository, PricingRepositoryImpl>();
            serviceCollection.AddSingleton<Func<IPricingRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IPricingRepository>());

            serviceCollection.AddSingleton<IExpressionSerializer, XmlExpressionSerializer>();

            serviceCollection.AddTransient<IPricingService, PricingServiceImpl>();
            serviceCollection.AddTransient<IPricingSearchService, PricingSearchServiceImpl>();
            serviceCollection.AddSingleton<IPricingExtensionManager, DefaultPricingExtensionManagerImpl>();
            serviceCollection.AddSingleton<PricingExportImport>();
            serviceCollection.AddSingleton<PolymorphicPricingJsonConverter>();
            serviceCollection.AddTransient<ProductPriceDocumentChangesProvider>();
            serviceCollection.AddTransient<ProductPriceDocumentBuilder>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _applicationBuilder = appBuilder;

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleSettings.AllSettings, ModuleInfo.Id);

            var modulePermissions = ModulePermissions.AllPermissions.Select(p => new Permission
            {
                Name = p,
                GroupName = "Pricing",
                ModuleId = ModuleInfo.Id
            }).ToArray();
            var permissionsRegistrar = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsRegistrar.RegisterPermissions(modulePermissions);

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<PricingDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

            // Next lines allow to use polymorph types in API controller methods
            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(appBuilder.ApplicationServices.GetService<PolymorphicPricingJsonConverter>());

            // Add price document source to the product indexing configuration
            // TODO: fix and uncomment this code
            //var productIndexingConfigurations = appBuilder.ApplicationServices.GetRequiredService<IndexDocumentConfiguration[]>();
            //if (productIndexingConfigurations != null)
            //{
            //    var productPriceDocumentSource = new IndexDocumentSource
            //    {
            //        ChangesProvider = appBuilder.ApplicationServices.GetRequiredService<ProductPriceDocumentChangesProvider>(),
            //        DocumentBuilder = appBuilder.ApplicationServices.GetRequiredService<ProductPriceDocumentBuilder>()
            //    };

            //    foreach (var configuration in productIndexingConfigurations.Where(c => c.DocumentType == KnownDocumentTypes.Product))
            //    {
            //        if (configuration.RelatedSources == null)
            //        {
            //            configuration.RelatedSources = new List<IndexDocumentSource>();
            //        }

            //        configuration.RelatedSources.Add(productPriceDocumentSource);
            //    }
            //}

            //Marketing expression
            // TODO: uncomment this code if Marketing module will become a part of the Pricing module (or remove if it won't)
            //var promotionExtensionManager = appBuilder.ApplicationServices.GetRequiredService<IMarketingExtensionManager>();

            //promotionExtensionManager.PromotionDynamicExpressionTree = GetPromotionDynamicExpression();
            //promotionExtensionManager.DynamicContentExpressionTree = GetContentDynamicExpression();

            //Pricing expression
            var pricingExtensionManager = appBuilder.ApplicationServices.GetRequiredService<IPricingExtensionManager>();
            pricingExtensionManager.ConditionExpressionTree = GetPricingDynamicExpression();
        }

        public void Uninstall()
        {
        }

        #endregion

        #region ISupportExportImportModule Members

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            var exportJob = _applicationBuilder.ApplicationServices.GetRequiredService<PricingExportImport>();
            await exportJob.DoExportAsync(outStream, progressCallback, cancellationToken);
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            var importJob = _applicationBuilder.ApplicationServices.GetRequiredService<PricingExportImport>();
            await importJob.DoImportAsync(inputStream, progressCallback, cancellationToken);
        }

        #endregion

        private static ConditionExpressionTree GetPricingDynamicExpression()
        {
            var conditions = new List<DynamicExpression>
            {
                new ConditionGeoTimeZone(), new ConditionGeoZipCode(), new ConditionStoreSearchedPhrase(), new ConditionAgeIs(), new ConditionGenderIs(),
                new ConditionGeoCity(), new ConditionGeoCountry(), new ConditionGeoState(), new ConditionLanguageIs(), new UserGroupsContainsCondition()
            };
            var rootBlock = new BlockPricingCondition { AvailableChildren = conditions };
            var retVal = new ConditionExpressionTree()
            {
                Children = new DynamicExpression[] { rootBlock }
            };
            return retVal;
        }

        private static ConditionExpressionTree GetContentDynamicExpression()
        {
            var conditions = new List<DynamicExpression>
            {
                new ConditionGeoTimeZone(), new ConditionGeoZipCode(), new ConditionStoreSearchedPhrase(), new ConditionAgeIs(), new ConditionGenderIs(),
                new ConditionGeoCity(), new ConditionGeoCountry(), new ConditionGeoState(), new ConditionLanguageIs()
            };
            var rootBlock = new BlockContentCondition { AvailableChildren = conditions };
            var retVal = new ConditionExpressionTree()
            {
                Children = new DynamicExpression[] { rootBlock }
            };
            return retVal;
        }

        private static PromoDynamicExpressionTree GetPromotionDynamicExpression()
        {
            var customerConditionBlock = new BlockCustomerCondition
            {
                AvailableChildren = new List<DynamicExpression>
                {
                    new ConditionIsEveryone(), new ConditionIsFirstTimeBuyer(),
                    new ConditionIsRegisteredUser(), new UserGroupsContainsCondition()
                }
            };

            var catalogConditionBlock = new BlockCatalogCondition
            {
                AvailableChildren = new List<DynamicExpression>
                {
                    new ConditionEntryIs(), new ConditionCurrencyIs(),
                    new ConditionCodeContains(), new ConditionCategoryIs(),
                    new ConditionInStockQuantity()
                }
            };

            var cartConditionBlock = new BlockCartCondition
            {
                AvailableChildren = new List<DynamicExpression>
                {
                    new ConditionCartSubtotalLeast(), new ConditionAtNumItemsInCart(),
                    new ConditionAtNumItemsInCategoryAreInCart(), new ConditionAtNumItemsOfEntryAreInCart()
                }
            };
            var rewardBlock = new RewardBlock
            {
                AvailableChildren = new List<DynamicExpression>
                {
                    new RewardCartGetOfAbsSubtotal(), new RewardCartGetOfRelSubtotal(), new RewardItemGetFreeNumItemOfProduct(), new RewardItemGetOfAbs(),
                    new RewardItemGetOfAbsForNum(), new RewardItemGetOfRel(), new RewardItemGetOfRelForNum(),
                    new RewardItemGiftNumItem(), new RewardShippingGetOfAbsShippingMethod(), new RewardShippingGetOfRelShippingMethod(), new RewardPaymentGetOfAbs(),
                    new RewardPaymentGetOfRel()
                }
            };

            var rootBlocks = new DynamicExpression[] { customerConditionBlock, catalogConditionBlock, cartConditionBlock, rewardBlock }.ToList();
            var retVal = new PromoDynamicExpressionTree
            {
                Children = rootBlocks,
            };
            return retVal;
        }
    }
}
