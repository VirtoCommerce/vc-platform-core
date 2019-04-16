using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.StoreModule.Data.Migrations
{
    public partial class UpdateStoreV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
	                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190409085836_InitialStore', '2.2.3-servicing-35854')

                        UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.StoreModule.Core.Model.Store' WHERE [Name] = 'FilteredBrowsing' AND ObjectType = 'VirtoCommerce.Domain.Store.Model.Store'
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
