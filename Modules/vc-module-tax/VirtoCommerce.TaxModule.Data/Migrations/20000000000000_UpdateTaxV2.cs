using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.TaxModule.Data.Migrations
{
    public partial class UpdateTaxV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.StoreModule.Data.Migrations.Configuration'))
                    BEGIN
	                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190409093711_InitialTax', '2.2.3-servicing-35854')
                        ALTER TABLE [StoreTaxProvider] ADD [TypeName] nvarchar(128) NOT NULL Default ('')
                    END");

            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
                        UPDATE StoreTaxProvider SET [TypeName] = 'FixedRateTaxProvider' WHERE [Code] = 'FixedRate'
				    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
