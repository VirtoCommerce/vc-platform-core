using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.PaymentModule.Data.Migrations
{
    public partial class UpdatePaymentV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
                        INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190508102043_InitialPayment', '2.2.3-servicing-35854')
                    END");

            migrationBuilder.AddColumn<string>(name: "TypeName", table: "StorePaymentMethod", maxLength: 128, nullable: true, defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
