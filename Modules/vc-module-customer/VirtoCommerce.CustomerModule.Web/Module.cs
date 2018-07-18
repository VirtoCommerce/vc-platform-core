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
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.CustomerModule.Core;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerModule.Data.ExportImport;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.CustomerModule.Data.Search;
using VirtoCommerce.CustomerModule.Data.Search.Indexing;
using VirtoCommerce.CustomerModule.Data.Services;
using VirtoCommerce.CustomerModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Repositories;
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
            serviceCollection.AddTransient<ICustomerRepository, CustomerRepositoryImpl>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Customer") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<CustomerDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<Func<ICustomerRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICustomerRepository>());
            serviceCollection.AddSingleton<Func<IMemberRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICustomerRepository>());
            serviceCollection.AddSingleton<IMemberService, CommerceMembersServiceImpl>();

            serviceCollection.AddSingleton<ISearchRequestBuilder, MemberSearchRequestBuilder>();
            serviceCollection.AddSingleton<IMemberSearchService, MemberSearchServiceDecorator>();
            var snapshot = serviceCollection.BuildServiceProvider();

            var memberIndexingConfiguration = new IndexDocumentConfiguration
            {
                DocumentType = KnownDocumentTypes.Member,
                DocumentSource = new IndexDocumentSource
                {
                    ChangesProvider = snapshot.GetService<MemberDocumentChangesProvider>(),
                    DocumentBuilder = snapshot.GetService<MemberDocumentBuilder>(),
                },
            };

            serviceCollection.AddSingleton(memberIndexingConfiguration);
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;
            AbstractTypeFactory<Member>.RegisterType<Organization>().MapToType<OrganizationDataEntity>();
            AbstractTypeFactory<Member>.RegisterType<Contact>().MapToType<ContactDataEntity>();
            AbstractTypeFactory<Member>.RegisterType<Vendor>().MapToType<VendorDataEntity>();
            AbstractTypeFactory<Member>.RegisterType<Employee>().MapToType<EmployeeDataEntity>();

            AbstractTypeFactory<MemberDataEntity>.RegisterType<ContactDataEntity>();
            AbstractTypeFactory<MemberDataEntity>.RegisterType<OrganizationDataEntity>();
            AbstractTypeFactory<MemberDataEntity>.RegisterType<VendorDataEntity>();
            AbstractTypeFactory<MemberDataEntity>.RegisterType<EmployeeDataEntity>();

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission() { GroupName = "Customer", Name = x }).ToArray());

            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicMemberJsonConverter());

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<CustomerDbContext>();
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
            return _appBuilder.ApplicationServices.GetRequiredService<CustomerExportImport>().ExportAsync(outStream, options, progressCallback, cancellationToken);
        }

        public Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<CustomerExportImport>().ImportAsync(inputStream, options, progressCallback, cancellationToken);
        }
    }
}
