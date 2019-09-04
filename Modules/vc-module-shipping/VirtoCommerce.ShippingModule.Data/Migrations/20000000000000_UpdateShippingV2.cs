using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.ShippingModule.Data.Migrations
{
    public partial class UpdateShippingV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.StoreModule.Data.Migrations.Configuration'))
                    BEGIN
                        INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190520140954_InitialShipping', '2.2.3-servicing-35854')
                        ALTER TABLE [StoreShippingMethod] ADD [TypeName] nvarchar(128) NOT NULL Default ('')
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
