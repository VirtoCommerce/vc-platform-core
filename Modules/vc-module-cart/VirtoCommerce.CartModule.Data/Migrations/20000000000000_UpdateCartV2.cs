using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CartModule.Data.Migrations
{
    public partial class UpdateCartV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.CartModule.Data.Migrations.Configuration'))
                    BEGIN
	                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190530172311_InitialCart', '2.2.3-servicing-35854')
                        ALTER TABLE [CartPayment] ADD [CreatedDate] datetime2 NOT NULL DEFAULT('0001-01-01 00:00:00')
                        ALTER TABLE [CartPayment] ADD [ModifiedDate] datetime2 NULL
                        ALTER TABLE [CartPayment] ADD [CreatedBy] nvarchar(64) NULL
                        ALTER TABLE [CartPayment] ADD [ModifiedBy] nvarchar(64) NULL
                        EXEC sp_RENAME 'CartShipment.FulfilmentCenterId' , 'FulfillmentCenterId', 'COLUMN'
                        EXEC sp_RENAME 'CartLineItem.FulfilmentLocationCode' , 'FulfillmentLocationCode', 'COLUMN'

                        UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.CartModule.Core.Model.LineItem'     WHERE ObjectType = 'VirtoCommerce.Domain.Cart.Model.LineItem'
                        UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.CartModule.Core.Model.Payment'      WHERE ObjectType = 'VirtoCommerce.Domain.Cart.Model.Payment'
                        UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.CartModule.Core.Model.Shipment'     WHERE ObjectType = 'VirtoCommerce.Domain.Cart.Model.Shipment'
                        UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.CartModule.Core.Model.ShoppingCart' WHERE ObjectType = 'VirtoCommerce.Domain.Cart.Model.ShoppingCart'

                        UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.CartModule.Core.Model.LineItem'     WHERE ObjectType = 'VirtoCommerce.Domain.Cart.Model.LineItem'
                        UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.CartModule.Core.Model.Payment'      WHERE ObjectType = 'VirtoCommerce.Domain.Cart.Model.Payment'
                        UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.CartModule.Core.Model.Shipment'     WHERE ObjectType = 'VirtoCommerce.Domain.Cart.Model.Shipment'
                        UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.CartModule.Core.Model.ShoppingCart' WHERE ObjectType = 'VirtoCommerce.Domain.Cart.Model.ShoppingCart'
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
