using Microsoft.Practices.Unity;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Http;
using VirtoCommerce.CoreModule.Data.Services;
using VirtoCommerce.Domain.Common;
using VirtoCommerce.Domain.Order.Events;
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Domain.Order.Services;
using VirtoCommerce.Domain.Payment.Services;
using VirtoCommerce.Domain.Shipping.Services;
using VirtoCommerce.OrderModule.Data.Handlers;
using VirtoCommerce.OrderModule.Data.Notifications;
using VirtoCommerce.OrderModule.Data.Repositories;
using VirtoCommerce.OrderModule.Data.Services;
using VirtoCommerce.OrderModule.Web.ExportImport;
using VirtoCommerce.OrderModule.Web.JsonConverters;
using VirtoCommerce.OrderModule.Web.Model;
using VirtoCommerce.OrderModule.Web.Resources;
using VirtoCommerce.OrderModule.Web.Security;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Notifications;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.OrderModule.Web
{
    public class Module : ModuleBase, ISupportExportImportModule
    {
        private readonly string _connectionString = ConfigurationHelper.GetConnectionStringValue("VirtoCommerce.Orders") ?? ConfigurationHelper.GetConnectionStringValue("VirtoCommerce");
        private readonly IUnityContainer _container; 

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        #region IModule Members

        public override void SetupDatabase()
        {
            using (var context = new OrderRepositoryImpl(_connectionString, _container.Resolve<AuditableInterceptor>()))
            {
                var initializer = new SetupDatabaseInitializer<OrderRepositoryImpl, Data.Migrations.Configuration>();
                initializer.InitializeDatabase(context);
            }
        }

        public override void Initialize()
        {
            var eventHandlerRegistrar = _container.Resolve<IHandlerRegistrar>();

            //Registration welcome email notification.
            eventHandlerRegistrar.RegisterHandler<OrderChangedEvent>(async (message, token) => await _container.Resolve<AdjustInventoryOrderChangedEventHandler>().Handle(message));
            eventHandlerRegistrar.RegisterHandler<OrderChangedEvent>(async (message, token) => await _container.Resolve<CancelPaymentOrderChangedEventHandler>().Handle(message));
            eventHandlerRegistrar.RegisterHandler<OrderChangedEvent>(async (message, token) => await _container.Resolve<LogChangesOrderChangedEventHandler>().Handle(message));
            eventHandlerRegistrar.RegisterHandler<OrderChangedEvent>(async (message, token) => await _container.Resolve<SendNotificationsOrderChangedEventHandler>().Handle(message));

            _container.RegisterType<IOrderRepository>(new InjectionFactory(c => new OrderRepositoryImpl(_connectionString, _container.Resolve<AuditableInterceptor>(), new EntityPrimaryKeyGeneratorInterceptor())));
            _container.RegisterType<IUniqueNumberGenerator, SequenceUniqueNumberGeneratorServiceImpl>();

            _container.RegisterType<ICustomerOrderService, CustomerOrderServiceImpl>();
            _container.RegisterType<ICustomerOrderSearchService, CustomerOrderServiceImpl>();
            _container.RegisterType<ICustomerOrderBuilder, CustomerOrderBuilderImpl>();

            _container.RegisterType<ICustomerOrderTotalsCalculator, DefaultCustomerOrderTotalsCalculator>(new ContainerControlledLifetimeManager());
        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            //Add order numbers formats settings  to store module allows to use individual number formats in each store
            var settingManager = _container.Resolve<ISettingsManager>();
            var numberFormatSettings = settingManager.GetModuleSettings("VirtoCommerce.Orders").Where(x => x.Name.EndsWith("NewNumberTemplate")).ToArray();
            settingManager.RegisterModuleSettings("VirtoCommerce.Store", numberFormatSettings);

            var notificationManager = _container.Resolve<INotificationManager>();
            notificationManager.RegisterNotificationType(() => new OrderCreateEmailNotification(_container.Resolve<IEmailNotificationSendingGateway>())
            {
                DisplayName = "Create order notification",
                Description = "This notification sends by email to client when he create order",
                NotificationTemplate = new NotificationTemplate
                {
                    Body = OrderNotificationResource.CreateOrderNotificationBody,
                    Subject = OrderNotificationResource.CreateOrderNotificationSubject,
                    Language = "en-US"
                }
            });

            notificationManager.RegisterNotificationType(() => new OrderPaidEmailNotification(_container.Resolve<IEmailNotificationSendingGateway>())
            {
                DisplayName = "Order paid notification",
                Description = "This notification sends by email to client when all payments of order has status paid",
                NotificationTemplate = new NotificationTemplate
                {
                    Body = OrderNotificationResource.OrderPaidNotificationBody,
                    Subject = OrderNotificationResource.OrderPaidNotificationSubject,
                    Language = "en-US"
                }
            });

            notificationManager.RegisterNotificationType(() => new OrderSentEmailNotification(_container.Resolve<IEmailNotificationSendingGateway>())
            {
                DisplayName = "Order sent notification",
                Description = "This notification sends by email to client when all shipments gets status sent",
                NotificationTemplate = new NotificationTemplate
                {
                    Body = OrderNotificationResource.OrderSentNotificationBody,
                    Subject = OrderNotificationResource.OrderSentNotificationSubject,
                    Language = "en-US"
                }
            });

            notificationManager.RegisterNotificationType(() => new NewOrderStatusEmailNotification(_container.Resolve<IEmailNotificationSendingGateway>())
            {
                DisplayName = "New order status notification",
                Description = "This notification sends by email to client when status of orders has been changed",
                NotificationTemplate = new NotificationTemplate
                {
                    Body = OrderNotificationResource.NewOrderStatusNotificationBody,
                    Subject = OrderNotificationResource.NewOrderStatusNotificatonSubject,
                    Language = "en-US"
                }
            });

            notificationManager.RegisterNotificationType(() => new CancelOrderEmailNotification(_container.Resolve<IEmailNotificationSendingGateway>())
            {
                DisplayName = "Cancel order notification",
                Description = "This notification sends by email to client when order canceled",
                NotificationTemplate = new NotificationTemplate
                {
                    Body = OrderNotificationResource.CancelOrderNotificationBody,
                    Subject = OrderNotificationResource.CancelOrderNotificationSubject,
                    Language = "en-US"
                }
            });

            notificationManager.RegisterNotificationType(() => new InvoiceEmailNotification(_container.Resolve<IEmailNotificationSendingGateway>())
            {
                Description = "The template for for customer order invoice (used for PDF generation)",
                DisplayName = "The invoice for customer order",
                NotificationTemplate = new NotificationTemplate
                {
                    Body = InvoiceResource.Body,
                    Subject = InvoiceResource.Subject,
                    Language = "en-US"
                }
            });

            var securityScopeService = _container.Resolve<IPermissionScopeService>();
            securityScopeService.RegisterSope(() => new OrderStoreScope());
            securityScopeService.RegisterSope(() => new OrderResponsibleScope());

            //Next lines allow to use polymorph types in API controller methods
            var httpConfiguration = _container.Resolve<HttpConfiguration>();
            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new PolymorphicOperationJsonConverter(_container.Resolve<IPaymentMethodsService>(), _container.Resolve<IShippingMethodsService>()));

            //Next lines need to correct XML serialization for orders models
            var allShippingmethodTypes = _container.Resolve<IShippingMethodsService>().GetAllShippingMethods().Select(x => x.GetType()).ToArray();
            var allPaymentMethodTypes = _container.Resolve<IPaymentMethodsService>().GetAllPaymentMethods().Select(x => x.GetType()).ToArray();
            var allOrderKnownTypes = new[] { typeof(Shipment), typeof(PaymentIn), typeof(CustomerOrder) }.Concat(allPaymentMethodTypes).Concat(allShippingmethodTypes).ToArray();
            httpConfiguration.Formatters.XmlFormatter.SetSerializer<CustomerOrder>(new DataContractSerializer(typeof(CustomerOrder), allOrderKnownTypes));
            httpConfiguration.Formatters.XmlFormatter.SetSerializer<CustomerOrderSearchResult>(new DataContractSerializer(typeof(CustomerOrderSearchResult), allOrderKnownTypes));
        }

        #endregion

        #region ISupportExportImportModule Members

        public void DoExport(System.IO.Stream outStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var job = _container.Resolve<OrderExportImport>();
            job.DoExport(outStream, progressCallback);
        }

        public void DoImport(System.IO.Stream inputStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var job = _container.Resolve<OrderExportImport>();
            job.DoImport(inputStream, progressCallback);
        }

        public string ExportDescription
        {
            get
            {
                var settingManager = _container.Resolve<ISettingsManager>();
                return settingManager.GetValue("Order.ExportImport.Description", string.Empty);
            }
        }

        #endregion
    }
}
