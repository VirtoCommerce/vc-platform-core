using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.MarketingModule.Core;
using VirtoCommerce.MarketingModule.Core.Events;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Handlers;
using VirtoCommerce.MarketingModule.Data.Promotions;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.MarketingModule.Data.Services;
using VirtoCommerce.MarketingModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Serialization;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.MarketingModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {


        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Marketing") ?? configuration.GetConnectionString("VirtoCommerce");

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


            var settingsManager = serviceCollection.BuildServiceProvider().GetRequiredService<ISettingsManager>();
            var promotionCombinePolicy = settingsManager.GetValue("Marketing.Promotion.CombinePolicy", "BestReward");
            if (promotionCombinePolicy.EqualsInvariant("CombineStackable"))
            {
                serviceCollection.AddSingleton<IMarketingPromoEvaluator, CombineStackablePromotionPolicy>();
            }
            else
            {
                serviceCollection.AddSingleton<IMarketingPromoEvaluator, BestRewardPromotionPolicy>();
            }

            AbstractTypeFactory<DynamicPromotion>.RegisterType<DynamicPromotion>().WithFactory(() => serviceCollection.BuildServiceProvider().GetService<DynamicPromotion>());
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.General.AllSettings, ModuleInfo.Id);

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

            //Next lines allow to use polymorph types in API controller methods
            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicMarketingJsonConverter(appBuilder.ApplicationServices.GetService<IMarketingExtensionManager>(),
                appBuilder.ApplicationServices.GetService<IExpressionSerializer>()));

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<MarketingDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
        }

        public Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
