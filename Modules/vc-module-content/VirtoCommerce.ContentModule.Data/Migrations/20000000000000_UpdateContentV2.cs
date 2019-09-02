using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.ContentModule.Data.Migrations
{
    public partial class UpdateContentV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.Content.Data.Migrations.Configuration'))
                    BEGIN
	                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20180629125447_InitialContent', '2.2.3-servicing-35854')

                        BEGIN
                            UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.ContentModule.Core.Model.FrontMatterHeaders' WHERE ObjectType = 'VirtoCommerce.ContentModule.Web.FrontMatterHeaders'
                        END

                        BEGIN
                            UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.ContentModule.Core.Model.FrontMatterHeaders' WHERE ObjectType = 'VirtoCommerce.ContentModule.Web.FrontMatterHeaders'
                        END
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
