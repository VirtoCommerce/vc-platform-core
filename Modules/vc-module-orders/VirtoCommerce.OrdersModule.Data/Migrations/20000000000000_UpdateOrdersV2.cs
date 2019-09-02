using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.OrdersModule.Data.Migrations
{
    public partial class UpdateOrdersV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.OrderModule.Data.Migrations.Configuration'))
                    BEGIN
                        INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190530163834_InitialOrders', '2.2.3-servicing-35854')

	                    BEGIN
                            ALTER TABLE [CustomerOrder] ADD [Status] nvarchar(64) NULL
                            ALTER TABLE [CustomerOrder] ADD [CreatedDate] datetime2 NOT NULL DEFAULT('0001-01-01 00:00:00')
                            ALTER TABLE [CustomerOrder] ADD [ModifiedDate] datetime2 NULL
                            ALTER TABLE [CustomerOrder] ADD [CreatedBy] nvarchar(64) NULL
                            ALTER TABLE [CustomerOrder] ADD [ModifiedBy] nvarchar(64) NULL
                            ALTER TABLE [CustomerOrder] ADD [Number] nvarchar(64) NOT NULL DEFAULT('')
                            ALTER TABLE [CustomerOrder] ADD [IsApproved] bit NOT NULL DEFAULT(0)
                            ALTER TABLE [CustomerOrder] ADD [Comment] nvarchar(2048) NULL
                            ALTER TABLE [CustomerOrder] ADD [Currency] nvarchar(3) NOT NULL DEFAULT('')
                            ALTER TABLE [CustomerOrder] ADD [Sum] money NOT NULL DEFAULT(0)
                            ALTER TABLE [CustomerOrder] ADD [IsCancelled] [bit] NOT NULL DEFAULT(0)
                            ALTER TABLE [CustomerOrder] ADD [CancelledDate] datetime2 NULL
                            ALTER TABLE [CustomerOrder] ADD [CancelReason] nvarchar(2048) NULL
		                    EXEC ('UPDATE [CustomerOrder] SET [Status] = oo.[Status], CreatedDate = oo.CreatedDate, ModifiedDate = oo.ModifiedDate,
                                CreatedBy = oo.CreatedBy, ModifiedBy = oo.ModifiedBy,
	                            Number = oo.Number, IsApproved = oo.IsApproved, Comment = oo.Comment, Currency = oo.Currency, [Sum] = oo.[Sum],
                                IsCancelled = oo.IsCancelled, CancelledDate = oo.CancelledDate, CancelReason = oo.CancelReason
                                FROM [CustomerOrder] co INNER JOIN OrderOperation oo ON co.Id = oo.Id')
	                    END

                        EXEC sp_RENAME 'OrderLineItem.FulfilmentLocationCode' , 'FulfillmentLocationCode', 'COLUMN'

                        BEGIN
                            ALTER TABLE [OrderPaymentIn] ADD [Status] nvarchar(64) NULL
                            ALTER TABLE [OrderPaymentIn] ADD [CreatedDate] datetime2 NOT NULL DEFAULT('0001-01-01 00:00:00')
                            ALTER TABLE [OrderPaymentIn] ADD [ModifiedDate] datetime2 NULL
                            ALTER TABLE [OrderPaymentIn] ADD [CreatedBy] nvarchar(64) NULL
                            ALTER TABLE [OrderPaymentIn] ADD [ModifiedBy] nvarchar(64) NULL
                            ALTER TABLE [OrderPaymentIn] ADD [Number] nvarchar(64) NOT NULL DEFAULT('')
                            ALTER TABLE [OrderPaymentIn] ADD [IsApproved] bit NOT NULL DEFAULT(0)
                            ALTER TABLE [OrderPaymentIn] ADD [Comment] nvarchar(2048) NULL
                            ALTER TABLE [OrderPaymentIn] ADD [Currency] nvarchar(3) NOT NULL DEFAULT('')
                            ALTER TABLE [OrderPaymentIn] ADD [Sum] money NOT NULL DEFAULT(0)
                            ALTER TABLE [OrderPaymentIn] ADD [IsCancelled] [bit] NOT NULL DEFAULT(0)
                            ALTER TABLE [OrderPaymentIn] ADD [CancelledDate] datetime2 NULL
                            ALTER TABLE [OrderPaymentIn] ADD [CancelReason] nvarchar(2048) NULL
		                    EXEC ('UPDATE [OrderPaymentIn] SET [Status] = oo.[Status], CreatedDate = oo.CreatedDate, ModifiedDate = oo.ModifiedDate,
                                    CreatedBy = oo.CreatedBy, ModifiedBy = oo.ModifiedBy, Number = oo.Number, IsApproved = oo.IsApproved,
                                    Comment = oo.Comment, Currency = oo.Currency, [Sum] = oo.[Sum], IsCancelled = oo.IsCancelled, 
	                                CancelledDate = oo.CancelledDate, CancelReason = oo.CancelReason
                                    FROM [OrderPaymentIn] co INNER JOIN OrderOperation oo ON co.Id = oo.Id')
	                    END
	                    
	                    BEGIN
                            ALTER TABLE [OrderShipment] ADD [Status] nvarchar(64) NULL
                            ALTER TABLE [OrderShipment] ADD [CreatedDate] datetime2 NOT NULL DEFAULT('0001-01-01 00:00:00')
                            ALTER TABLE [OrderShipment] ADD [ModifiedDate] datetime2 NULL
                            ALTER TABLE [OrderShipment] ADD [CreatedBy] nvarchar(64) NULL
                            ALTER TABLE [OrderShipment] ADD [ModifiedBy] nvarchar(64) NULL
                            ALTER TABLE [OrderShipment] ADD [Number] nvarchar(64) NOT NULL DEFAULT('')
                            ALTER TABLE [OrderShipment] ADD [IsApproved] bit NOT NULL DEFAULT(0)
                            ALTER TABLE [OrderShipment] ADD [Comment] nvarchar(2048) NULL
                            ALTER TABLE [OrderShipment] ADD [Currency] nvarchar(3) NOT NULL DEFAULT('')
                            ALTER TABLE [OrderShipment] ADD [Sum] money NOT NULL DEFAULT(0)
                            ALTER TABLE [OrderShipment] ADD [IsCancelled] [bit] NOT NULL DEFAULT(0)
                            ALTER TABLE [OrderShipment] ADD [CancelledDate] datetime2 NULL
                            ALTER TABLE [OrderShipment] ADD [CancelReason] nvarchar(2048) NULL
		                    EXEC ('UPDATE [OrderShipment] SET [Status] = oo.[Status], CreatedDate = oo.CreatedDate, ModifiedDate = oo.ModifiedDate,
                                    CreatedBy = oo.CreatedBy, ModifiedBy = oo.ModifiedBy, Number = oo.Number, IsApproved = oo.IsApproved,
                                    Comment = oo.Comment, Currency = oo.Currency, [Sum] = oo.[Sum], IsCancelled = oo.IsCancelled, 
	                                CancelledDate = oo.CancelledDate, CancelReason = oo.CancelReason
                                    FROM [OrderPaymentIn] co INNER JOIN OrderOperation oo ON co.Id = oo.Id')
	                    END
                        BEGIN
                             ALTER TABLE [dbo].[CustomerOrder] DROP CONSTRAINT [FK_dbo.CustomerOrder_dbo.OrderOperation_Id]
                             ALTER TABLE [dbo].[OrderPaymentIn] DROP CONSTRAINT [FK_dbo.OrderPaymentIn_dbo.OrderOperation_Id]
                             ALTER TABLE [dbo].[OrderShipment] DROP CONSTRAINT [FK_dbo.OrderShipment_dbo.OrderOperation_Id]
                        END

                        BEGIN
                            UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.OrderModule.Core.Model.CustomerOrder'  WHERE ObjectType = 'VirtoCommerce.Domain.Order.Model.CustomerOrder'
                            UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.OrderModule.Core.Model.LineItem'       WHERE ObjectType = 'VirtoCommerce.Domain.Order.Model.LineItem'
                            UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.OrderModule.Core.Model.PaymentIn'      WHERE ObjectType = 'VirtoCommerce.Domain.Order.Model.PaymentIn'
                            UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.OrderModule.Core.Model.Shipment'       WHERE ObjectType = 'VirtoCommerce.Domain.Order.Model.Shipment'

                            UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.OrderModule.Core.Model.CustomerOrder'   WHERE ObjectType = 'VirtoCommerce.Domain.Order.Model.CustomerOrder'
                            UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.OrderModule.Core.Model.LineItem'        WHERE ObjectType = 'VirtoCommerce.Domain.Order.Model.LineItem'
                            UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.OrderModule.Core.Model.PaymentIn'       WHERE ObjectType = 'VirtoCommerce.Domain.Order.Model.PaymentIn'
                            UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.OrderModule.Core.Model.Shipment'        WHERE ObjectType = 'VirtoCommerce.Domain.Order.Model.Shipment'
                        END

                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
