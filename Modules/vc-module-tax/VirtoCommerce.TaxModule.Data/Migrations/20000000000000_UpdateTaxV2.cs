using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.TaxModule.Data.Migrations
{
    public partial class UpdateTaxV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
	                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190409093711_InitialTax', '2.2.3-servicing-35854')
                    END");

            migrationBuilder.AddColumn<string>(name: "TypeName", table: "StoreTaxProvider", maxLength: 128, nullable: true, defaultValue: "");
            migrationBuilder.Sql("  UPDATE StoreTaxProvider SET [TypeName] = 'FixedTaxRateProvider' WHERE [Code] = 'FixedRate'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
