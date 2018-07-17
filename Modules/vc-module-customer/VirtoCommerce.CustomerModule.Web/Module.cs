using System;
using System.Web.Http;
using Microsoft.Practices.Unity;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.CustomerModule.Data.Search;
using VirtoCommerce.CustomerModule.Data.Search.Indexing;
using VirtoCommerce.CustomerModule.Data.Services;
using VirtoCommerce.CustomerModule.Web.ExportImport;
using VirtoCommerce.CustomerModule.Web.JsonConverters;
using VirtoCommerce.Domain.Customer.Events;
using VirtoCommerce.Domain.Customer.Model;
using VirtoCommerce.Domain.Customer.Services;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.CustomerModule.Web
{
    public class Module : ModuleBase, ISupportExportImportModule
    {
        private readonly string _connectionString = ConfigurationHelper.GetConnectionStringValue("VirtoCommerce.Customer") ?? ConfigurationHelper.GetConnectionStringValue("VirtoCommerce");
        private readonly IUnityContainer _container; 

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        #region IModule Members

        public override void SetupDatabase()
        {
            using (var db = new CustomerRepositoryImpl(_connectionString, _container.Resolve<AuditableInterceptor>()))
            {
                var initializer = new SetupDatabaseInitializer<CustomerRepositoryImpl, Data.Migrations.Configuration>();
                initializer.InitializeDatabase(db);
            }

        }

        public override void Initialize()
        {
            base.Initialize();

            Func<CustomerRepositoryImpl> customerRepositoryFactory = () => new CustomerRepositoryImpl(_connectionString, new EntityPrimaryKeyGeneratorInterceptor(), _container.Resolve<AuditableInterceptor>(),
                new ChangeLogInterceptor(_container.Resolve<Func<IPlatformRepository>>(), ChangeLogPolicy.Cumulative, new[] { nameof(MemberDataEntity) }));

            _container.RegisterInstance<Func<ICustomerRepository>>(customerRepositoryFactory);
            _container.RegisterInstance<Func<IMemberRepository>>(customerRepositoryFactory);

            _container.RegisterType<IMemberService, CommerceMembersServiceImpl>();

            // Indexed search
            _container.RegisterType<ISearchRequestBuilder, MemberSearchRequestBuilder>(nameof(MemberSearchRequestBuilder));
            _container.RegisterType<IMemberSearchService, MemberSearchServiceDecorator>();
        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            AbstractTypeFactory<Member>.RegisterType<Organization>().MapToType<OrganizationDataEntity>();
            AbstractTypeFactory<Member>.RegisterType<Contact>().MapToType<ContactDataEntity>();
            AbstractTypeFactory<Member>.RegisterType<Vendor>().MapToType<VendorDataEntity>();
            AbstractTypeFactory<Member>.RegisterType<Employee>().MapToType<EmployeeDataEntity>();

            AbstractTypeFactory<MemberDataEntity>.RegisterType<ContactDataEntity>();
            AbstractTypeFactory<MemberDataEntity>.RegisterType<OrganizationDataEntity>();
            AbstractTypeFactory<MemberDataEntity>.RegisterType<VendorDataEntity>();
            AbstractTypeFactory<MemberDataEntity>.RegisterType<EmployeeDataEntity>();

            //Next lines allow to use polymorph types in API controller methods
            var httpConfiguration = _container.Resolve<HttpConfiguration>();
            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new PolymorphicMemberJsonConverter());

            // Indexing configuration
            var memberIndexingConfiguration = new IndexDocumentConfiguration
            {
                DocumentType = KnownDocumentTypes.Member,
                DocumentSource = new IndexDocumentSource
                {
                    ChangesProvider = _container.Resolve<MemberDocumentChangesProvider>(),
                    DocumentBuilder = _container.Resolve<MemberDocumentBuilder>(),
                },
            };

            _container.RegisterInstance(memberIndexingConfiguration.DocumentType, memberIndexingConfiguration);
        }

        #endregion

        #region ISupportExportImportModule Members

        public void DoExport(System.IO.Stream outStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var exportJob = _container.Resolve<CustomerExportImport>();
            exportJob.DoExport(outStream, progressCallback);
        }

        public void DoImport(System.IO.Stream inputStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var exportJob = _container.Resolve<CustomerExportImport>();
            exportJob.DoImport(inputStream, progressCallback);
        }

        public string ExportDescription
        {
            get
            {
                var settingManager = _container.Resolve<ISettingsManager>();
                return settingManager.GetValue("Customer.ExportImport.Description", string.Empty);
            }
        }
        #endregion
    }
}
