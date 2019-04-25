using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CartModule.Data.Migrations
{
    public partial class UpdateCartV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
	                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190410111538_InitialCart', '2.2.3-servicing-35854')
                        ALTER TABLE [CartPayment] ADD [CreatedDate] datetime2 NOT NULL DEFAULT('0001-01-01 00:00:00')
                        ALTER TABLE [CartPayment] ADD [ModifiedDate] datetime2 NULL
                        ALTER TABLE [CartPayment] ADD [CreatedBy] nvarchar(64) NULL
                        ALTER TABLE [CartPayment] ADD [ModifiedBy] nvarchar(64) NULL
                        EXEC sp_RENAME 'CartShipment.FulfilmentCenterId' , 'FulfillmentCenterId', 'COLUMN'
                        EXEC sp_RENAME 'CartLineItem.FulfilmentLocationCode' , 'FulfillmentLocationCode', 'COLUMN'
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
