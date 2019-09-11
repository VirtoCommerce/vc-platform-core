using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.LicensingModule.Data.Migrations
{
    public partial class UpdateLicensingV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.LicensingModule.Data.Migrations.Configuration'))
                    BEGIN
	                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190320150046_InitialLicensing', '2.2.3-servicing-35854')
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
