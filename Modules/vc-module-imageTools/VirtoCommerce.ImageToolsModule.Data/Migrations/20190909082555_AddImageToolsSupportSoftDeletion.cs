using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.ImageToolsModule.Data.Migrations
{
    public partial class AddImageToolsSupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ThumbnailTask",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ThumbnailTask");
        }
    }
}
