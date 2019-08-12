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
using VirtoCommerce.CustomerModule.Core;
using VirtoCommerce.CustomerModule.Core.Events;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerModule.Core.Services.Indexed;
using VirtoCommerce.CustomerModule.Data.ExportImport;
using VirtoCommerce.CustomerModule.Data.Handlers;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.CustomerModule.Data.Search;
using VirtoCommerce.CustomerModule.Data.Search.Indexing;
using VirtoCommerce.CustomerModule.Data.Services;
using VirtoCommerce.CustomerModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.CustomerModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        private IApplicationBuilder _appBuilder;

        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            serviceCollection.AddTransient<ICustomerRepository, CustomerRepository>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Customer") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<CustomerDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<Func<ICustomerRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICustomerRepository>());
            serviceCollection.AddSingleton<Func<IMemberRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICustomerRepository>());

            serviceCollection.AddSingleton<ISearchRequestBuilder, MemberSearchRequestBuilder>();
            serviceCollection.AddTransient<IIndexedMemberSearchService, MemberIndexedSearchService>();
            serviceCollection.AddTransient<IMemberSearchService, MemberSearchService>();
            serviceCollection.AddTransient<IMemberService, MemberService>();
            serviceCollection.AddSingleton<CustomerExportImport>();

            serviceCollection.AddSingleton<MemberDocumentChangesProvider>();
            serviceCollection.AddSingleton<MemberDocumentBuilder>();

            serviceCollection.AddSingleton(provider => new IndexDocumentConfiguration
            {
                DocumentType = KnownDocumentTypes.Member,
                DocumentSource = new IndexDocumentSource
                {
                    ChangesProvider = provider.GetService<MemberDocumentChangesProvider>(),
                    DocumentBuilder = provider.GetService<MemberDocumentBuilder>(),
                },
            });

            serviceCollection.AddTransient<LogChangesEventHandler>();
            serviceCollection.AddTransient<SecurtityAccountChangesEventHandler>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;
            AbstractTypeFactory<Member>.RegisterType<Organization>().MapToType<OrganizationEntity>();
            AbstractTypeFactory<Member>.RegisterType<Contact>().MapToType<ContactEntity>();
            AbstractTypeFactory<Member>.RegisterType<Vendor>().MapToType<VendorEntity>();
            AbstractTypeFactory<Member>.RegisterType<Employee>().MapToType<EmployeeEntity>();

            AbstractTypeFactory<MemberEntity>.RegisterType<ContactEntity>();
            AbstractTypeFactory<MemberEntity>.RegisterType<OrganizationEntity>();
            AbstractTypeFactory<MemberEntity>.RegisterType<VendorEntity>();
            AbstractTypeFactory<MemberEntity>.RegisterType<EmployeeEntity>();

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var dynamicPropertyRegistrar = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<Organization>();
            dynamicPropertyRegistrar.RegisterType<Contact>();
            dynamicPropertyRegistrar.RegisterType<Vendor>();
            dynamicPropertyRegistrar.RegisterType<Employee>();


            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission() { GroupName = "Customer", Name = x }).ToArray());

            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicMemberJsonConverter());

            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<MemberChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<LogChangesEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<UserChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<SecurtityAccountChangesEventHandler>().Handle(message));
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<CustomerDbContext>();
                dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

        }

        public void Uninstall()
        {
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<CustomerExportImport>().ExportAsync(outStream, options, progressCallback, cancellationToken);
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<CustomerExportImport>().ImportAsync(inputStream, options, progressCallback, cancellationToken);
        }
    }
}
