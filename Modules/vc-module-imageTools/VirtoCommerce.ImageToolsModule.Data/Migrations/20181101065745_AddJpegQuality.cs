using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.ImageToolsModule.Data.Migrations
{
    public partial class AddJpegQuality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JpegQuality",
                table: "ThumbnailOption",
                maxLength: 64,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JpegQuality",
                table: "ThumbnailOption");
        }
    }
}
